// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyntaxEvaluation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
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