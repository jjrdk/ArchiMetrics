namespace ArchiMeter.CodeReview.Rules
{
	using System.Linq;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class EmptyDoErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.DoStatement; }
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var whileStatement = (DoStatementSyntax)node;

			var sleepLoopFound = whileStatement.DescendantNodes()
			                                   .OfType<BlockSyntax>()
			                                   .Any(s => !s.ChildNodes().Any());

			if (sleepLoopFound)
			{
				var snippet = FindMethodParent(node).ToFullString();

				return new EvaluationResult
					       {
						       Comment = "Empty do loop found in code.", 
						       Quality = CodeQuality.Incompetent, 
						       QualityAttribute = QualityAttribute.CodeQuality | QualityAttribute.Testability, 
						       Snippet = snippet
					       };
			}

			return null;
		}
	}
}