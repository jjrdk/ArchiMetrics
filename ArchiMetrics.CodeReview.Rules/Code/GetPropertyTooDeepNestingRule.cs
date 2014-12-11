// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetPropertyTooDeepNestingRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GetPropertyTooDeepNestingRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using Microsoft.CodeAnalysis.CSharp;

	internal class GetPropertyTooDeepNestingRule : PropertyTooDeepNestingRule
	{
		public override string ID
		{
			get
			{
				return "AMC0012";
			}
		}

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.GetAccessorDeclaration;
			}
		}

		protected override string NestingMember
		{
			get
			{
				return "Property Getter";
			}
		}
	}
}