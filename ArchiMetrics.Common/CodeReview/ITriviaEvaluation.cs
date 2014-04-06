namespace ArchiMetrics.Common.CodeReview
{
	using Microsoft.CodeAnalysis;

	public interface ITriviaEvaluation : IEvaluation
	{
		EvaluationResult Evaluate(SyntaxTrivia trivia);
	}
}