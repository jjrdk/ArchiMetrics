// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellChecker.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SpellChecker type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
	using System;
	using System.IO;
	using System.Linq;
	using Analysis.Common.CodeReview;
	using Ionic.Zip;
	using NHunspell;

	internal class SpellChecker : ISpellChecker
	{
		private readonly IKnownPatterns _knownPatterns;
		private readonly Hunspell _speller;

		public SpellChecker(IKnownPatterns knownPatterns)
		{
			_knownPatterns = knownPatterns;
			using (var dictFile = ZipFile.Read(@"Dictionaries\dict-en.oxt"))
			{
				var affStream = new MemoryStream();
				var dicStream = new MemoryStream();
				dictFile.FirstOrDefault(z => z.FileName == "en_US.aff").Extract(affStream);
				dictFile.FirstOrDefault(z => z.FileName == "en_US.dic").Extract(dicStream);
				_speller = new Hunspell(affStream.ToArray(), dicStream.ToArray());
			}
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