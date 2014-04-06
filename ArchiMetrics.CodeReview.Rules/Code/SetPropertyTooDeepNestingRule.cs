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