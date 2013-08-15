// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportUtils.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReportUtils type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview
{
	using System.Linq;

	public static class ReportUtils
	{
		private static readonly string[] KnownTestAttributes = { "Test", "TestCase", "TestMethod", "Fact", "Theory" };

		internal static bool IsKnownTestAttribute(this string text)
		{
			return KnownTestAttributes.Contains(text);
		}
	}
}
