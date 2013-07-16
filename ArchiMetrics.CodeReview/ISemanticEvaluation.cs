namespace ArchiMetrics.CodeReview
{
	using Common;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public interface ISemanticEvaluation : IEvaluation
	{
		EvaluationResult Evaluate(SyntaxNode node, ISemanticModel semanticModel, ISolution solution);
	}
}