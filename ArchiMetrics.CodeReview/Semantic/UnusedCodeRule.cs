namespace ArchiMetrics.CodeReview.Semantic
{
	using System.Linq;
	using System.Threading;
	using Common;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;
	using Rules;

	internal abstract class UnusedCodeRule : EvaluationBase, ISemanticEvaluation
	{
		public EvaluationResult Evaluate(SyntaxNode node, ISemanticModel semanticModel, ISolution solution)
		{
			var symbol = semanticModel.GetDeclaredSymbol(node);
			var callers = symbol.FindCallers(solution, CancellationToken.None);

			if (!callers.Any())
			{
				return new EvaluationResult
					   {
						   Comment = "Uncalled code",
						   ImpactLevel = ImpactLevel.Member,
						   Namespace = GetCompilationUnitNamespace(node.GetLocation()
							   .SourceTree.GetRoot()),
						   Quality = CodeQuality.Incompetent,
						   QualityAttribute = QualityAttribute.CodeQuality,
						   Snippet = node.ToFullString()
					   };
			}

			return null;
		}
	}
}
