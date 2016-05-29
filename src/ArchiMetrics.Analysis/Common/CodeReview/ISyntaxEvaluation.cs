// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyntaxEvaluation.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ISyntaxEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.CodeReview
{
    using Microsoft.CodeAnalysis.CSharp;

    public interface ISyntaxEvaluation : IEvaluation
	{
		SyntaxKind EvaluatedKind { get; }
	}
}