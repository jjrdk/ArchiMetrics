// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LawOfDemeterViolationRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LawOfDemeterViolationRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Rules
{
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class LawOfDemeterViolationRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.MemberAccessExpression; }
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			var expression = memberAccess.Expression as MemberAccessExpressionSyntax;
			if (expression != null)
			{
				var syntax = expression.Expression;
				if (!(syntax is ThisExpressionSyntax))
				{
					return new EvaluationResult
						   {
							   Comment = "Violation of Law of Demeter", 
							   Quality = CodeQuality.NeedsReview, 
							   QualityAttribute = QualityAttribute.Modifiability | QualityAttribute.Reusability | QualityAttribute.Maintainability, 
							   Snippet = node.ToFullString()
						   };
				}
			}

			return null;
		}
	}
}