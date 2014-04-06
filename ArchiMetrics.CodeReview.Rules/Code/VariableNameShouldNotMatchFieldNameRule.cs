namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class VariableNameShouldNotMatchFieldNameRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.SimpleAssignmentExpression;
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

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsReview;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.CodeQuality | QualityAttribute.Maintainability;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Member;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var assignment = (BinaryExpressionSyntax)node;
			var left = assignment.Left as MemberAccessExpressionSyntax;
			if (left == null || !left.Expression.IsKind(SyntaxKind.ThisExpression))
			{
				return null;
			}

			var variable = left.Name as IdentifierNameSyntax;
			var right = assignment.Right as IdentifierNameSyntax;
			if (right == null || variable == null)
			{
				return null;
			}

			if (variable.Identifier.ValueText == right.Identifier.ValueText)
			{
				return new EvaluationResult
					   {
						   Snippet = assignment.ToFullString()
					   };
			}

			return null;
		}
	}
}