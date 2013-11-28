namespace ArchiMetrics.CodeReview.Rules.Code
{
	using Roslyn.Compilers.CSharp;

	internal class SetPropertyTooDeepNestingRule : PropertyTooDeepNestingRule
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.SetAccessorDeclaration;
			}
		}
	}
}