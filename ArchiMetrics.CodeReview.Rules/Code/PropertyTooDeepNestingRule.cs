// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyTooDeepNestingRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PropertyTooDeepNestingRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using Roslyn.Compilers.CSharp;

	internal class PropertyTooDeepNestingRule : TooDeepNestingRuleBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.PropertyDeclaration;
			}
		}

		protected override BlockSyntax GetBody(SyntaxNode node)
		{
			return null;
		}
	}
}