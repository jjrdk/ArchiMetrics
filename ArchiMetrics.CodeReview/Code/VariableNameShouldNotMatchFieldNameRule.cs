namespace ArchiMetrics.CodeReview.Code
{
	using Common;
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
						   QualityAttribute = QualityAttribute.CodeQuality | QualityAttribute.Maintainability
					   };
			}

			return null;
		}
	}
}