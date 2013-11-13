// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportUtils.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReportUtils type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules
{
	using System.Linq;
	using System.Text.RegularExpressions;

	public static class ReportUtils
	{
		private static readonly Regex CapitalRegex = new Regex("[A-Z]", RegexOptions.Compiled);
		private static readonly string[] KnownTestAttributes = { "Test", "TestCase", "TestMethod", "Fact", "Theory" };

		internal static bool IsKnownTestAttribute(this string text)
		{
			return KnownTestAttributes.Contains(text);
		}

		internal static string ToTitleCase(this string input)
		{
			return CapitalRegex.Replace(input, m => " " + m).Trim();
		}
	}
}
