namespace ArchiCop.UI
{
	using ArchiMeter.Common;

	using NHunspell;

	internal class SpellChecker : ISpellChecker
	{
		private readonly Hunspell _speller;

		public SpellChecker(Hunspell speller)
		{
			this._speller = speller;
		}

		public bool Spell(string word)
		{
			return this._speller.Spell(word);
		}
	}
}