// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISymbolEvaluation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
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