namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;

	internal class ExemptPatterns : IKnownPatterns
	{
		private readonly IList<Regex> _innerRegexes = new List<Regex>();

		public bool IsExempt(string word)
		{
			return _innerRegexes.Any(x => x.IsMatch(word));
		}

		public void Add(params string[] patterns)
		{
			foreach (var pattern in patterns.WhereNotNullOrWhitespace())
			{
				_innerRegexes.Add(new Regex(pattern, RegexOptions.Compiled));
			}
		}

		public void Remove(string pattern)
		{
			var toRemove = _innerRegexes.Where(x => x.ToString().Equals(pattern)).ToArray();
			foreach (var regex in toRemove)
			{
				_innerRegexes.Remove(regex);
			}
		}

		public void Clear()
		{
			_innerRegexes.Clear();
		}
	}
}