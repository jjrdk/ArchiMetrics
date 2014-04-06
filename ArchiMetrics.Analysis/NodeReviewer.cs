namespace ArchiMetrics.Analysis
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
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
								   let compilation = project.GetCompilationAsync()
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
			Task<Compilation> compilationTask,
			Solution solution)
		{
			var tree = await treeTask;
			var root = await tree.GetRootAsync();
			if (root == null)
			{
				return Enumerable.Empty<EvaluationResult>();
			}

			var compilation = await compilationTask;
			return await Inspect(filePath, projectName, root, compilation.GetSemanticModel(tree), solution);
		}

		private class InnerInspector : CSharpSyntaxVisitor<Task<IEnumerable<EvaluationResult>>>
		{
			private readonly IDictionary<SyntaxKind, IEvaluation[]> _evaluations;
			private readonly SemanticModel _model;
			private readonly Solution _solution;

			public InnerInspector(IDictionary<SyntaxKind, IEvaluation[]> evaluations, SemanticModel model, Solution solution)
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

				var baseResultTasks = await Task.WhenAll(node.ChildNodesAndTokens().Select(VisitNodeOrToken));
				var baseResults = baseResultTasks.SelectMany(x => x);
				if (_evaluations.ContainsKey(node.CSharpKind()))
				{
					var nodeEvaluations = _evaluations[node.CSharpKind()];
					var semmanticResults = await GetSemanticEvaluations(node, nodeEvaluations.OfType<ISemanticEvaluation>(), _model, _solution);

					var codeResults = GetCodeEvaluations(node, nodeEvaluations.OfType<ICodeEvaluation>());

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

			public virtual async Task<IEnumerable<EvaluationResult>> VisitToken(SyntaxToken token)
			{
				var tasks = await Task.WhenAll(token.LeadingTrivia.Concat(token.TrailingTrivia).Select(VisitTrivia));
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
								return await x.Evaluate(node, model, solution);
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
				if (_evaluations.ContainsKey(trivia.CSharpKind()))
				{
					var nodeEvaluations = _evaluations[trivia.CSharpKind()];
					return await GetTriviaEvaluations(trivia, nodeEvaluations.OfType<ITriviaEvaluation>());
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
