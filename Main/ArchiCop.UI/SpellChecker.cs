namespace ArchiMeter.UI
{
	using System;

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

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SpellChecker()
		{
			// Simply call Dispose(false).
			this.Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if(isDisposing)
			{
				//Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
				this._speller.Dispose(true);
			}
		}
	}
}
