namespace ArchiMetrics.Common.CodeReview
{
	using System;

	public interface ISpellChecker : IDisposable
	{
		bool Spell(string word);
	}
}
