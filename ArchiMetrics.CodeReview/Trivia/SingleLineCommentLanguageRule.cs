namespace ArchiMetrics.CodeReview.Trivia
{
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class SingleLineCommentLanguageRule : CommentLanguageRuleBase
	{
		public SingleLineCommentLanguageRule(ISpellChecker spellChecker)
			: base(spellChecker)
		{
		}

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.SingleLineCommentTrivia; }
		}
	}
}
