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
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class NodeReviewer : INodeInspector
	{
		private readonly Dictionary<SyntaxKind, IEvaluation[]> _evaluations;

		public NodeReviewer(IEnumerable<IEvaluation> evaluations)
		{
			_evaluations = evaluations.GroupBy(x => x.EvaluatedKind).ToDictionary(x => x.Key, x => x.ToArray());
		}

		public virtual async Task<IEnumerable<EvaluationResult>> Inspect(string projectPath, SyntaxNode node, ISemanticModel semanticModel, ISolution solution)
		{
			var inspector = new InnerInspector(_evaluations, semanticModel, solution);

			var inspectionTasks = await inspector.Visit(node);
			var inspectionResults = inspectionTasks.ToArray();
			foreach (var result in inspectionResults)
			{
				result.ProjectPath = projectPath;
			}

			return inspectionResults.AsEnumerable();
		}

		private class InnerInspector : SyntaxVisitor<Task<IEnumerable<EvaluationResult>>>
		{
			private readonly IDictionary<SyntaxKind, IEvaluation[]> _evaluations;
			private readonly ISemanticModel _model;
			private readonly ISolution _solution;

			public InnerInspector(IDictionary<SyntaxKind, IEvaluation[]> evaluations, ISemanticModel model, ISolution solution)
			{
				_evaluations = evaluations;
				_model = model;
				_solution = solution;
			}

			public override async Task<IEnumerable<EvaluationResult>> Visit(SyntaxNode node)
			{
				if (node == null)
				{
					return Enumerable.Empty<EvaluationResult>();
				}

				var baseResultTasks = await Task.WhenAll(node.ChildNodes().Select(Visit));
				var baseResults = baseResultTasks.SelectMany(x => x);
				if (_evaluations.ContainsKey(node.Kind))
				{
					var nodeEvaluations = _evaluations[node.Kind];
					var codeResults = GetCodeEvaluations(node, nodeEvaluations.OfType<ICodeEvaluation>());
					var semmanticResults = GetSemanticEvaluations(node, nodeEvaluations.OfType<ISemanticEvaluation>(), _model, _solution);

					return codeResults.Concat(semmanticResults).Concat(baseResults).ToArray();
				}

				return baseResults.ToArray();
			}

			public override async Task<IEnumerable<EvaluationResult>> DefaultVisit(SyntaxNode node)
			{
				var tasks =
					node.ChildNodesAndTokens()
						.Select(VisitNodeOrToken)
						.Concat(node.DescendantTrivia(x => x == node).Select(VisitTrivia));
				return (await Task.WhenAll(tasks)).SelectMany(x => x);
			}

			private Task<IEnumerable<EvaluationResult>> VisitNodeOrToken(SyntaxNodeOrToken nodeOrToken)
			{
				var node = nodeOrToken.AsNode();
				return node == null ? VisitToken(nodeOrToken.AsToken()) : Visit(node);
			}

			public virtual async Task<IEnumerable<EvaluationResult>> VisitToken(SyntaxToken token)
			{
				var tasks = await Task.WhenAll(token.LeadingTrivia.Concat(token.TrailingTrivia).Select(VisitTrivia));
				return tasks.SelectMany(x => x);
			}

			private async Task<IEnumerable<EvaluationResult>> VisitTrivia(SyntaxTrivia trivia)
			{
				if (_evaluations.ContainsKey(trivia.Kind))
				{
					var nodeEvaluations = _evaluations[trivia.Kind];
					return await GetTriviaEvaluations(trivia, nodeEvaluations.OfType<ITriviaEvaluation>());
				}

				return Enumerable.Empty<EvaluationResult>();
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

			private static IEnumerable<EvaluationResult> GetSemanticEvaluations(SyntaxNode node, IEnumerable<ISemanticEvaluation> nodeEvaluations, ISemanticModel model, ISolution solution)
			{
				if (model == null || solution == null)
				{
					return Enumerable.Empty<EvaluationResult>();
				}

				var results = nodeEvaluations
					.Select(x =>
						{
							try
							{
								return x.Evaluate(node, model, solution);
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
		}
	}
}
