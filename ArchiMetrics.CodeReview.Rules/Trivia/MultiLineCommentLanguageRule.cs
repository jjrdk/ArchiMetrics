namespace ArchiMetrics.CodeReview.Rules.Trivia
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis.CSharp;

	internal class MultiLineCommentLanguageRule : CommentLanguageRuleBase
	{
		public MultiLineCommentLanguageRule(ISpellChecker spellChecker)
			: base(spellChecker)
		{
		}

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.MultiLineCommentTrivia; }
		}
	}
}
