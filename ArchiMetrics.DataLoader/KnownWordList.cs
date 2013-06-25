namespace ArchiMetrics.DataLoader
{
	using System.Collections.Generic;
	using Common;

	public class KnownWordList : IKnownWordList
	{
		private static readonly HashSet<string> KnownStrings = new HashSet<string> { };

		public bool IsExempt(string word)
		{
			return KnownStrings.Contains(word);
		}
	}
}
