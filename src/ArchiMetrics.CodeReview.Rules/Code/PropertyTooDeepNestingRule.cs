// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyTooDeepNestingRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal abstract class PropertyTooDeepNestingRule : TooDeepNestingRuleBase
	{
		protected override BlockSyntax GetBody(SyntaxNode node)
		{
			var property = (AccessorDeclarationSyntax)node;
			return property.Body;
		}
	}
}