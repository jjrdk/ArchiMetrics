// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownPatterns.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the KnownPatterns type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;

	public class KnownPatterns : IKnownPatterns
	{
		private static readonly HashSet<Regex> KnownPatternsSet = new HashSet<Regex> { };

		public bool IsExempt(string word)
		{
			return KnownPatternsSet.Any(r => r.IsMatch(word));
		}

		public void Add(params string[] patterns)
		{
			foreach (var pattern in patterns)
			{
				KnownPatternsSet.Add(new Regex(pattern, RegexOptions.Compiled));
			}
		}

		public void Clear()
		{
			KnownPatternsSet.Clear();
		}
	}
}
