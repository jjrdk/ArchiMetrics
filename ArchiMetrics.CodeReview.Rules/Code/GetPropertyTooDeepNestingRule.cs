namespace ArchiMetrics.CodeReview.Rules.Code
{
	using Roslyn.Compilers.CSharp;

	internal class GetPropertyTooDeepNestingRule : PropertyTooDeepNestingRule
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.GetAccessorDeclaration;
			}
		}
	}
}