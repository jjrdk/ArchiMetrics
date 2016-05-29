// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISemanticEvaluation.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ISemanticEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.CodeReview
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;

    public interface ISemanticEvaluation : ISyntaxEvaluation
	{
		Task<EvaluationResult> Evaluate(SyntaxNode node, SemanticModel semanticModel, Solution solution);
	}
}