namespace ArchiMeter.DataLoader
{
	using Common;
	using NHunspell;

	internal class SpellChecker : ISpellChecker
	{
		private readonly Hunspell _speller;

		public SpellChecker(Hunspell speller)
		{
			_speller = speller;
		}

		public bool Spell(string word)
		{
			return _speller.Spell(word);
		}
	}
}