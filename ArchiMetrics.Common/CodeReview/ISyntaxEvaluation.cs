namespace ArchiMetrics.Common.CodeReview
{
	using Microsoft.CodeAnalysis.CSharp;

	public interface ISyntaxEvaluation : IEvaluation
	{
		SyntaxKind EvaluatedKind { get; }
	}
}