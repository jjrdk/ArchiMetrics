// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnusedGetPropertyRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnusedGetPropertyRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using Microsoft.CodeAnalysis.CSharp;

	internal class UnusedGetPropertyRule : UnusedCodeRule
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.GetAccessorDeclaration; }
		}
	}
}