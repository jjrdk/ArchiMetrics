// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the Extensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;

	public static class Extensions
	{
		private static readonly Regex CapitalRegex = new Regex("[A-Z]", RegexOptions.Compiled);
		private static readonly string[] KnownTestAttributes = { "Test", "TestCase", "TestMethod", "Fact", "Theory" };

		public static bool IsKnownTestAttribute(this string text)
		{
			return KnownTestAttributes.Contains(text);
		}

		public static void DisposeNotNull(this IDisposable disposable)
		{
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		public static string ToTitleCase(this string input)
		{
			return CapitalRegex.Replace(input, m => " " + m).Replace("_", " ").Trim();
		}

		public static async Task<T> FirstMatch<T>(this IEnumerable<Task<T>> tasks, Func<T, bool> predicate)
		{
			var finished = await Task.WhenAny(tasks);
			if (predicate(finished.Result))
			{
				return finished.Result;
			}

			var remaining = tasks.Except(new[] { finished }).ToArray();
			if (remaining.Length == 0)
			{
				return default(T);
			}

			return await FirstMatch(remaining, predicate);
		}
	}
}
