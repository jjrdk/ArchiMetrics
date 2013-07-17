namespace ArchiMetrics.CodeReview
{
	using Roslyn.Compilers.CSharp;

	public interface IEvaluation
	{
		SyntaxKind EvaluatedKind { get; }
	}
}