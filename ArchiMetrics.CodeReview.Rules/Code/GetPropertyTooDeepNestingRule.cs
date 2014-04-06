namespace ArchiMetrics.CodeReview.Rules.Code
{
	using Microsoft.CodeAnalysis.CSharp;

	internal class GetPropertyTooDeepNestingRule : PropertyTooDeepNestingRule
	{
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