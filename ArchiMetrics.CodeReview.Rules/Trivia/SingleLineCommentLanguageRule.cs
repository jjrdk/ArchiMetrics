namespace ArchiMetrics.CodeReview.Rules.Trivia
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis.CSharp;

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
