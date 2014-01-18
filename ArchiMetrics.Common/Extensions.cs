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
	using System.Linq;
	using System.Text.RegularExpressions;

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
	}
}
