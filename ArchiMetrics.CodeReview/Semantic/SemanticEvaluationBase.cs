namespace ArchiMetrics.CodeReview.Semantic
{
	using ArchiMetrics.CodeReview.Code;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;
	using Roslyn.Services;

	internal abstract class SemanticEvaluationBase : EvaluationBase, ISemanticEvaluation
	{
		public EvaluationResult Evaluate(SyntaxNode node, ISemanticModel semanticModel, ISolution solution)
		{
			var result = EvaluateImpl(node, semanticModel, solution);
			if (result != null)
			{
				var sourceTree = node.GetLocation().SourceTree;
				var filePath = sourceTree.FilePath;
				var unitNamespace = GetCompilationUnitNamespace(sourceTree.GetRoot());
				result.Namespace = unitNamespace;
				result.FilePath = filePath;
				result.LinesOfCodeAffected = this.GetLinesOfCode(result.Snippet);
			}

			return result;
		}

		protected abstract EvaluationResult EvaluateImpl(SyntaxNode node, ISemanticModel semanticModel, ISolution solution);
	}
}