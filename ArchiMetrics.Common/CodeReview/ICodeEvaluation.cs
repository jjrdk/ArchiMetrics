namespace ArchiMetrics.Common.CodeReview
{
	using Microsoft.CodeAnalysis;

	public interface ICodeEvaluation : IEvaluation
	{
		EvaluationResult Evaluate(SyntaxNode node);
	}
}
