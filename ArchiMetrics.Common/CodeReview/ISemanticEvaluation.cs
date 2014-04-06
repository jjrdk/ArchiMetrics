// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISemanticEvaluation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ISemanticEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.CodeReview
{
	using System.Threading.Tasks;
	using Microsoft.CodeAnalysis;

	public interface ISemanticEvaluation : IEvaluation
	{
		Task<EvaluationResult> Evaluate(SyntaxNode node, SemanticModel semanticModel, Solution solution);
	}
}