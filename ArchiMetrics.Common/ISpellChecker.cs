namespace ArchiMetrics.Common
{
	using System;

	public interface ISpellChecker : IDisposable
	{
		bool Spell(string word);
	}
}
