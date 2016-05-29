// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISymbolEvaluation.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ISymbolEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.CodeReview
{
    using Microsoft.CodeAnalysis;

    public interface ISymbolEvaluation : IEvaluation
	{
		SymbolKind EvaluatedKind { get; }

		EvaluationResult Evaluate(ISymbol symbol, SemanticModel semanticModel);
	}
}