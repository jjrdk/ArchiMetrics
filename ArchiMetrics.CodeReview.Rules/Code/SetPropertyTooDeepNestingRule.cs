// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetPropertyTooDeepNestingRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SetPropertyTooDeepNestingRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using Microsoft.CodeAnalysis.CSharp;

	internal class SetPropertyTooDeepNestingRule : PropertyTooDeepNestingRule
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.SetAccessorDeclaration;
			}
		}

		protected override string NestingMember
		{
			get
			{
				return "Property Setter";
			}
		}
	}
}