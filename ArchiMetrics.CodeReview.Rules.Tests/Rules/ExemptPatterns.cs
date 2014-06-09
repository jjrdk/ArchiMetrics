// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExemptPatterns.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ExemptPatterns type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
	using System.Collections;
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

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<string> GetEnumerator()
		{
			return _innerRegexes.Select(x => x.ToString())
				.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}