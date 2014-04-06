namespace ArchiMetrics.Common.CodeReview
{
	using Microsoft.CodeAnalysis.CSharp;

	public interface IEvaluation
	{
		SyntaxKind EvaluatedKind { get; }

		string Title { get; }

		string Suggestion { get; }

		CodeQuality Quality { get; }

		QualityAttribute QualityAttribute { get; }

		ImpactLevel ImpactLevel { get; }
	}
}