namespace ArchiMetrics.CodeReview
{
	using Common;
	using Roslyn.Compilers.CSharp;

	public interface ITriviaEvaluation : IEvaluation
	{
		EvaluationResult Evaluate(SyntaxTrivia trivia);
	}
}