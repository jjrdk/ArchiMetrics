// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeReviewer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NodeReviewer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;

	public class NodeReviewer : INodeInspector
	{
		private readonly Dictionary<SyntaxKind, IEvaluation[]> _evaluations;

		public NodeReviewer(IEnumerable<IEvaluation> evaluations)
		{
			_evaluations = evaluations.GroupBy(x => x.EvaluatedKind).ToDictionary(x => x.Key, x => x.ToArray());
		}

		public async Task<IEnumerable<EvaluationResult>> Inspect(Solution solution)
		{
			if (solution == null)
			{
				return Enumerable.Empty<EvaluationResult>();
			}

			var inspectionTasks = (from project in solution.Projects
								   where project.HasDocuments
								   let compilation = new Lazy<Compilation>(() => project.GetCompilationAsync().Result, LazyThreadSafetyMode.ExecutionAndPublication)
								   from doc in project.Documents.ToArray()
								   let tree = doc.GetSyntaxTreeAsync()
								   select GetInspections(project.FilePath, project.Name, tree, compilation, solution))
						   .ToArray();

			var results = await Task.WhenAll(inspectionTasks);

			return results.SelectMany(x => x).ToArray();
		}

		public async Task<IEnumerable<EvaluationResult>> Inspect(string projectPath, string projectName, SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var inspector = new InnerInspector(_evaluations, semanticModel, solution);

			var inspectionTasks = await inspector.Visit(node);
			var inspectionResults = inspectionTasks.ToArray();
			foreach (var result in inspectionResults)
			{
				result.ProjectName = projectName;
				result.ProjectPath = projectPath;
			}

			return inspectionResults.AsEnumerable();
		}

		private async Task<IEnumerable<EvaluationResult>> GetInspections(
			string filePath,
			string projectName,
			Task<SyntaxTree> treeTask,
			Lazy<Compilation> compilationTask,
			Solution solution)
		{
			var tree = await treeTask;
			var root = await tree.GetRootAsync();
			if (root == null)
			{
				return Enumerable.Empty<EvaluationResult>();
			}

			var compilation = compilationTask.Value;
			return await Inspect(filePath, projectName, root, compilation.GetSemanticModel(tree), solution);
		}

		private class InnerInspector : CSharpSyntaxVisitor<Task<IEnumerable<EvaluationResult>>>
		{
			private readonly IDictionary<SyntaxKind, ITriviaEvaluation[]> _triviaEvaluations;
			private readonly IDictionary<SyntaxKind, ICodeEvaluation[]> _codeEvaluations;
			private readonly IDictionary<SyntaxKind, ISemanticEvaluation[]> _semanticEvaluations;
			private readonly SemanticModel _model;
			private readonly Solution _solution;

			public InnerInspector(IDictionary<SyntaxKind, IEvaluation[]> evaluations, SemanticModel model, Solution solution)
			{
				foreach (var evaluation in evaluations)
				{
					_triviaEvaluations.Add(evaluation.Key, evaluation.Value.OfType<ITriviaEvaluation>().ToArray());
					_codeEvaluations.Add(evaluation.Key, evaluation.Value.OfType<ICodeEvaluation>().ToArray());
					_semanticEvaluations.Add(evaluation.Key, evaluation.Value.OfType<ISemanticEvaluation>().ToArray());
				}

				foreach (var kind in _triviaEvaluations.Where(x => x.Value.Length == 0).Select(x => x.Key).ToArray())
				{
					_triviaEvaluations.Remove(kind);
				}

				foreach (var kind in _codeEvaluations.Where(x => x.Value.Length == 0).Select(x => x.Key).ToArray())
				{
					_codeEvaluations.Remove(kind);
				}

				foreach (var kind in _semanticEvaluations.Where(x => x.Value.Length == 0).Select(x => x.Key).ToArray())
				{
					_semanticEvaluations.Remove(kind);
				}

				_model = model;
				_solution = solution;
			}

			public override async Task<IEnumerable<EvaluationResult>> Visit(SyntaxNode node)
			{
				if (node == null)
				{
					return Enumerable.Empty<EvaluationResult>();
				}

				var baseResultTasks = await Task.WhenAll(node.ChildNodesAndTokens().Select(VisitNodeOrToken)).ConfigureAwait(false);
				var baseResults = baseResultTasks.SelectMany(x => x);
				var codeResults = Enumerable.Empty<EvaluationResult>();
				var semanticResults = Enumerable.Empty<EvaluationResult>();
				var kind = node.CSharpKind();
				if (_codeEvaluations.ContainsKey(kind))
				{
					codeResults = GetCodeEvaluations(node, _codeEvaluations[kind]);
				}

				if (_semanticEvaluations.ContainsKey(kind))
				{
					semanticResults = await GetSemanticEvaluations(node, _semanticEvaluations[kind], _model, _solution).ConfigureAwait(false);
				}

				return codeResults.Concat(semanticResults).Concat(baseResults).ToArray();
			}

			public override async Task<IEnumerable<EvaluationResult>> DefaultVisit(SyntaxNode node)
			{
				var tasks =
					node.ChildNodesAndTokens()
						.Select(VisitNodeOrToken)
						.Concat(node.DescendantTrivia(x => x == node).Select(VisitTrivia));
				return (await Task.WhenAll(tasks).ConfigureAwait(false)).SelectMany(x => x);
			}

			public virtual async Task<IEnumerable<EvaluationResult>> VisitToken(SyntaxToken token)
			{
				var tasks = await Task.WhenAll(token.LeadingTrivia.Concat(token.TrailingTrivia).Select(VisitTrivia)).ConfigureAwait(false);
				return tasks.SelectMany(x => x);
			}

			private static Task<IEnumerable<EvaluationResult>> GetTriviaEvaluations(SyntaxTrivia trivia, IEnumerable<ITriviaEvaluation> nodeEvaluations)
			{
				return Task.Factory.StartNew(
					() =>
					{
						var results = nodeEvaluations.Select(
							x =>
							{
								try
								{
									return x.Evaluate(trivia);
								}
								catch (Exception ex)
								{
									return new EvaluationResult
											   {
												   Title = ex.Message,
												   Suggestion = ex.StackTrace,
												   ErrorCount = 1,
												   Snippet = trivia.ToFullString(),
												   Quality = CodeQuality.Broken
											   };
								}
							})
								.Where(x => x != null && x.Quality != CodeQuality.Good)
								.ToArray();
						return results.AsEnumerable();
					});
			}

			private static IEnumerable<EvaluationResult> GetCodeEvaluations(SyntaxNode node, IEnumerable<ICodeEvaluation> nodeEvaluations)
			{
				var results = nodeEvaluations
					.Select(x =>
					{
						try
						{
							return x.Evaluate(node);
						}
						catch (Exception ex)
						{
							return new EvaluationResult
							{
								Title = ex.Message,
								Suggestion = ex.StackTrace,
								ErrorCount = 1,
								Snippet = node.ToFullString(),
								Quality = CodeQuality.Broken
							};
						}
					})
					.Where(x => x != null && x.Quality != CodeQuality.Good)
					.ToArray();
				return results;
			}

			private static async Task<IEnumerable<EvaluationResult>> GetSemanticEvaluations(SyntaxNode node, IEnumerable<ISemanticEvaluation> nodeEvaluations, SemanticModel model, Solution solution)
			{
				if (model == null || solution == null)
				{
					return Enumerable.Empty<EvaluationResult>();
				}

				var tasks = nodeEvaluations
					.Select(async x =>
						{
							try
							{
								return await x.Evaluate(node, model, solution).ConfigureAwait(false);
							}
							catch (Exception ex)
							{
								return new EvaluationResult
										   {
											   Title = ex.Message,
											   Suggestion = ex.StackTrace,
											   ErrorCount = 1,
											   Snippet = node.ToFullString(),
											   Quality = CodeQuality.Broken
										   };
							}
						});
				var results = (await Task.WhenAll(tasks))
					.Where(x => x != null && x.Quality != CodeQuality.Good)
					.ToArray();
				return results;
			}

			private async Task<IEnumerable<EvaluationResult>> VisitTrivia(SyntaxTrivia trivia)
			{
				var kind = trivia.CSharpKind();
				if (_triviaEvaluations.ContainsKey(kind))
				{
					return await GetTriviaEvaluations(trivia, _triviaEvaluations[kind]).ConfigureAwait(false);
				}

				return Enumerable.Empty<EvaluationResult>();
			}

			private Task<IEnumerable<EvaluationResult>> VisitNodeOrToken(SyntaxNodeOrToken nodeOrToken)
			{
				var node = nodeOrToken.AsNode();
				return node == null ? VisitToken(nodeOrToken.AsToken()) : Visit(node);
			}
		}
	}
}
