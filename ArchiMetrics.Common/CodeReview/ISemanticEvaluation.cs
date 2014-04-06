namespace ArchiMetrics.Common.CodeReview
{
	using System.Threading.Tasks;
	using Microsoft.CodeAnalysis;

	public interface ISemanticEvaluation : IEvaluation
	{
		Task<EvaluationResult> Evaluate(SyntaxNode node, SemanticModel semanticModel, Solution solution);
	}
}