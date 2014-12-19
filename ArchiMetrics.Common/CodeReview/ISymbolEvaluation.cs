namespace ArchiMetrics.Common.CodeReview
{
	using Microsoft.CodeAnalysis;

	public interface ISymbolEvaluation : IEvaluation
	{
		SymbolKind EvaluatedKind { get; }

		EvaluationResult Evaluate(ISymbol symbol, SemanticModel semanticModel);
	}
}