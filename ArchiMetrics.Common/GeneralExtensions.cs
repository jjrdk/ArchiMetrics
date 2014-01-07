// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneralExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GeneralExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System.Linq;

	public static class GeneralExtensions
	{
		private static readonly string[] KnownTestAttributes = { "Test", "TestCase", "TestMethod", "Fact", "Theory" };

		public static bool IsKnownTestAttribute(this string text)
		{
			return KnownTestAttributes.Contains(text);
		}
	}
}