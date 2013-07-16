namespace ArchiMetrics.CodeReview.Semantic
{
	using Roslyn.Compilers.CSharp;

	internal class UnusedEventRule : UnusedCodeRule
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.EventDeclaration; }
		}
	}
}