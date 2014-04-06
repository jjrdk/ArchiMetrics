// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellChecker.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SpellChecker type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI
{
	using System;
	using ArchiMetrics.Common.CodeReview;
	using NHunspell;

	internal class SpellChecker : ISpellChecker
	{
		private readonly IKnownPatterns _knownPatterns;
		private readonly Hunspell _speller;

		public SpellChecker(Hunspell speller, IKnownPatterns knownPatterns)
		{
			_speller = speller;
			_knownPatterns = knownPatterns;
		}

		~SpellChecker()
		{
			Dispose(false);
		}

		public bool Spell(string word)
		{
			return _knownPatterns.IsExempt(word) || _speller.Spell(word);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_speller.Dispose(true);
			}
		}
	}
}
