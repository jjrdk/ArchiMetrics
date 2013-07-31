// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISemanticEvaluation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ISemanticEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview
{
	using Common;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public interface ISemanticEvaluation : IEvaluation
	{
		EvaluationResult Evaluate(SyntaxNode node, ISemanticModel semanticModel, ISolution solution);
	}
}