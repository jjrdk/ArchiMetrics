// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameSpellingRuleBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NameSpellingRuleBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Code
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;
	using ArchiMetrics.Common;

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
