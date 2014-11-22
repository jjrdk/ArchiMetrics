// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeReviewer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;

	public class NodeReviewer : INodeInspector
	{
		private readonly Dictionary<SyntaxKind, ITriviaEvaluation[]> _triviaEvaluations;
		private readonly Dictionary<SyntaxKind, ICodeEvaluation[]> _codeEvaluations;
		private readonly Dictionary<SyntaxKind, ISemanticEvaluation[]> _semanticEvaluations;
		private readonly SyntaxKind[] _allSyntaxKinds;

		public NodeReviewer(IEnumerable<IEvaluation> evaluations)
		{
			var allEvaluations = evaluations.ToArray();
			_allSyntaxKinds = allEvaluations.Select(x => x.EvaluatedKind)
				.Distinct()
				.ToArray();
			_triviaEvaluations = allEvaluations.OfType<ITriviaEvaluation>().GroupBy(x => x.EvaluatedKind).ToDictionary(x => x.Key, x => x.ToArray());
			_codeEvaluations = allEvaluations.OfType<ICodeEvaluation>().GroupBy(x => x.EvaluatedKind).ToDictionary(x => x.Key, x => x.ToArray());
			_semanticEvaluations = allEvaluations.OfType<ISemanticEvaluation>().GroupBy(x => x.EvaluatedKind).ToDictionary(x => x.Key, x => x.ToArray());
		}

		public async Task<IEnumerable<EvaluationResult>> Inspect(Solution solution)
		{
			if (solution == null)
			{
				return Enumerable.Empty<EvaluationResult>();
			}

			var dataTasks = (from project in solution.Projects
							 where project.HasDocuments
							 from doc in project.Documents.ToArray()
							 let model = doc.GetSemanticModelAsync()
							 let root = doc.GetSyntaxRootAsync()
							 select new { FilePath = project.FilePath, Name = project.Name, Model = model, Root = root, Solution = solution })
							 .ToArray();
			await Task.WhenAll(dataTasks.Select(x => x.Model).Concat(dataTasks.Select(x => (Task)x.Root))).ConfigureAwait(false);

			var inspectionTasks = dataTasks.Select(x => GetInspections(x.FilePath, x.Name, x.Model.Result, x.Root.Result, x.Solution));
			var results = await Task.WhenAll(inspectionTasks).ConfigureAwait(false);

			return results.SelectMany(x => x).ToArray();
		}

		public async Task<IEnumerable<EvaluationResult>> Inspect(string projectPath, string projectName, SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var inspector = new InnerInspector(_allSyntaxKinds, _triviaEvaluations, _codeEvaluations, _semanticEvaluations, semanticModel, solution);

			var inspectionTasks = await inspector.Visit(node).ConfigureAwait(false);
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
			SemanticModel model,
			SyntaxNode root,
			Solution solution)
		{
			if (root == null)
			{
				return Enumerable.Empty<EvaluationResult>();
			}

			return await Inspect(filePath, projectName, root, model, solution).ConfigureAwait(false);
		}

		private class InnerInspector : CSharpSyntaxVisitor<Task<IEnumerable<EvaluationResult>>>
		{
			private readonly IList<SyntaxKind> _supportedSyntaxKinds;
			private readonly IDictionary<SyntaxKind, ITriviaEvaluation[]> _triviaEvaluations;
			private readonly IDictionary<SyntaxKind, ICodeEvaluation[]> _codeEvaluations;
			private readonly IDictionary<SyntaxKind, ISemanticEvaluation[]> _semanticEvaluations;
			private readonly SemanticModel _model;
			private readonly Solution _solution;

			public InnerInspector(IList<SyntaxKind> supportedSyntaxKinds, IDictionary<SyntaxKind, ITriviaEvaluation[]> triviaEvaluations, IDictionary<SyntaxKind, ICodeEvaluation[]> codeEvaluations, IDictionary<SyntaxKind, ISemanticEvaluation[]> semanticEvaluations, SemanticModel model, Solution solution)
			{
				_supportedSyntaxKinds = supportedSyntaxKinds;
				_triviaEvaluations = triviaEvaluations;
				_codeEvaluations = codeEvaluations;
				_semanticEvaluations = semanticEvaluations;
				_model = model;
				_solution = solution;
			}

			public override async Task<IEnumerable<EvaluationResult>> Visit(SyntaxNode node)
			{
				if (node == null)
				{
					return Enumerable.Empty<EvaluationResult>();
				}

				var nodeChecks = CheckNodes(node.DescendantNodesAndSelf().Where(x => x.CSharpKind().In(_supportedSyntaxKinds)).ToArray());
				var tokenResultTasks = node.DescendantTokens().Select(VisitToken);
				var nodeResultTasks = await Task.WhenAll(nodeChecks).ConfigureAwait(false);

				var baseResults = nodeResultTasks.SelectMany(x => x).Concat(tokenResultTasks.SelectMany(x => x));
				return baseResults;
			}

			public override Task<IEnumerable<EvaluationResult>> DefaultVisit(SyntaxNode node)
			{
				return Task.FromResult(Enumerable.Empty<EvaluationResult>());
			}

			protected virtual IEnumerable<EvaluationResult> VisitToken(SyntaxToken token)
			{
				var results = token.LeadingTrivia.Concat(token.TrailingTrivia)
					.Where(x => _triviaEvaluations.ContainsKey(x.CSharpKind()))
					.SelectMany(trivia => GetTriviaEvaluations(trivia, _triviaEvaluations[trivia.CSharpKind()]));

				return results;
			}

			private static IEnumerable<EvaluationResult> GetTriviaEvaluations(SyntaxTrivia trivia, IEnumerable<ITriviaEvaluation> nodeEvaluations)
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
				return results;
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
				var results = (await Task.WhenAll(tasks).ConfigureAwait(false))
					.Where(x => x != null && x.Quality != CodeQuality.Good)
					.ToArray();
				return results;
			}

			private async Task<IEnumerable<EvaluationResult>> CheckNodes(SyntaxNode[] nodes)
			{
				var semanticResultTasks = nodes.Where(x => _semanticEvaluations.ContainsKey(x.CSharpKind()))
					.Select(x => CheckSemantics(x, x.CSharpKind()));
				var codeResults = nodes.Where(x => _codeEvaluations.ContainsKey(x.CSharpKind()))
					.SelectMany(x => CheckCode(x, x.CSharpKind()));
				var semanticResults = await Task.WhenAll(semanticResultTasks).ConfigureAwait(false);

				return semanticResults.SelectMany(x => x).Concat(codeResults);
			}

			private IEnumerable<EvaluationResult> CheckCode(SyntaxNode node, SyntaxKind kind)
			{
				var codeResults = GetCodeEvaluations(node, _codeEvaluations[kind]);
				return codeResults;
			}

			private async Task<IEnumerable<EvaluationResult>> CheckSemantics(SyntaxNode node, SyntaxKind kind)
			{
				var semanticResults = await GetSemanticEvaluations(node, _semanticEvaluations[kind], _model, _solution).ConfigureAwait(false);

				return semanticResults;
			}
		}
	}
}
