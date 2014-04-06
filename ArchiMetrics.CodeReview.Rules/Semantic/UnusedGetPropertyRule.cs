namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using Microsoft.CodeAnalysis.CSharp;

	internal class UnusedGetPropertyRule : UnusedCodeRule
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.GetAccessorDeclaration; }
		}
	}
}