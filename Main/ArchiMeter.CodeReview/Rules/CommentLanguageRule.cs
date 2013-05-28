namespace ArchiMeter.CodeReview.Rules
{
	using System.Linq;
	using ArchiMeter.Common;
	using Roslyn.Compilers.CSharp;

	internal abstract class CommentLanguageRuleBase : TriviaEvaluationBase
	{
		private readonly ISpellChecker _spellChecker;

		public CommentLanguageRuleBase(ISpellChecker spellChecker)
		{
			_spellChecker = spellChecker;
		}

		protected override EvaluationResult EvaluateImpl(SyntaxTrivia node)
		{
			var commentWords = node.ToFullString().Trim('/', '*').Trim().Split(' ');
			var errorCount = commentWords.Aggregate(0, (i, s) => i + (_spellChecker.Spell(s) ? 0 : 1));
			if (errorCount >= 0.50 * commentWords.Length)
			{
				return new EvaluationResult
						   {
							   Comment = "Suspicious language comment",
							   ErrorCount = 1,
							   ImpactLevel = ImpactLevel.Member,
							   Quality = CodeQuality.NeedsReview,
							   QualityAttribute = QualityAttribute.Maintainability | QualityAttribute.Conformance,
							   Snippet = node.ToFullString()
						   };
			}

			return null;
		}
	}

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