namespace ArchiMeter.CodeReview.Rules
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;
	using Common;

	internal abstract class NameSpellingRuleBase : CodeEvaluationBase
	{
		private static readonly Regex CapitalRegex = new Regex("[A-Z]", RegexOptions.Compiled);
		private readonly IKnownWordList _knownWordList;
		private readonly ISpellChecker _speller;

		public NameSpellingRuleBase(ISpellChecker speller, IKnownWordList knownWordList)
		{
			_speller = speller;
			_knownWordList = knownWordList;
		}

		protected bool IsSpelledCorrectly(string name)
		{
			var wordParts = CapitalRegex.Replace(name, m => " " + m)
				.Trim()
				.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
				.Where(s => !_knownWordList.IsExempt(s));
			return wordParts.Aggregate(true, (b, s) => b && _speller.Spell(s));
		}
	}
}
