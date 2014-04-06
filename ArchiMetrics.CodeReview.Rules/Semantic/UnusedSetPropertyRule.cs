namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using Microsoft.CodeAnalysis.CSharp;

	internal class UnusedSetPropertyRule : UnusedCodeRule
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.SetAccessorDeclaration; }
		}
	}
}