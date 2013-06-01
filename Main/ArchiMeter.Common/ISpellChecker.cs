namespace ArchiMeter.Common
{
	using System;

	public interface ISpellChecker : IDisposable
	{
		bool Spell(string word);
	}
}
