namespace ArchiMetrics.Analysis.Model
{
	using System.Collections.Concurrent;
	using System.Text.RegularExpressions;

	internal abstract class TransformerBase
	{
		private readonly ConcurrentDictionary<string, Regex> _regexes = new ConcurrentDictionary<string, Regex>();

		protected Regex GetTransform(string pattern)
		{
			return _regexes.GetOrAdd(pattern, x => new Regex(x, RegexOptions.Compiled));
		}
	}
}