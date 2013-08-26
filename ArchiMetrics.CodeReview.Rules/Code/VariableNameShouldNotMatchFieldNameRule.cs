// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VariableNameShouldNotMatchFieldNameRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the VariableNameShouldNotMatchFieldNameRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class VariableNameShouldNotMatchFieldNameRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.AssignExpression;
			}
		}

		public override string Title
		{
			get
			{
				return "Variable Name Should Not Match Field Name";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Rename variable to avoid confusion with assigned field.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var assignment = (BinaryExpressionSyntax)node;
			var left = assignment.Left as MemberAccessExpressionSyntax;
			if (left == null || left.Expression.Kind != SyntaxKind.ThisExpression)
			{
				return null;
			}

			var variable = left.Name as IdentifierNameSyntax;
			var right = assignment.Right as IdentifierNameSyntax;
			if (right == null || variable == null)
			{
				return null;
			}

			if (variable.IsEquivalentTo(right))
			{
				return new EvaluationResult
					   {
						   Comment = "Suspicious field assignment.", 
						   ImpactLevel = ImpactLevel.Member, 
						   Quality = CodeQuality.NeedsReview, 
						   QualityAttribute = QualityAttribute.CodeQuality | QualityAttribute.Maintainability,
						   Snippet = assignment.ToFullString()
					   };
			}

			return null;
		}
	}
}