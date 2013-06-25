namespace ArchiMeter.CodeReview.Rules
{
	using Common;
	using Roslyn.Compilers.CSharp;

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