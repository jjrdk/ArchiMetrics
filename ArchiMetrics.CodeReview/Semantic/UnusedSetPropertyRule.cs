namespace ArchiMetrics.CodeReview.Semantic
{
	using Roslyn.Compilers.CSharp;

	internal class UnusedSetPropertyRule : UnusedCodeRule
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.SetAccessorDeclaration; }
		}
	}
}