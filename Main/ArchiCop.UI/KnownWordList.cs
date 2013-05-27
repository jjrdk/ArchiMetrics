namespace ArchiCop.UI
{
	using System.Collections.Generic;

	using ArchiMeter.Common;

	public class KnownWordList : IKnownWordList
	{
		private static readonly HashSet<string> KnownStrings = new HashSet<string> { };

		public bool IsExempt(string word)
		{
			return KnownStrings.Contains(word);
		}
	}
}