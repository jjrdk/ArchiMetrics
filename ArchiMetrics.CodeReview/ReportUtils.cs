// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportUtils.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReportUtils type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview
{
	using System;
	using System.Globalization;
	using System.Linq;
	using Common;

	public static class ReportUtils
	{
		private static readonly string[] TestNames = new[]
										  {
											"Tests", 
											"TestInterceptor", 
											"Testing", 
											"SchedulerTest", 
											"Demo", 
											"UnitTest", 
											"Mock", 
											"DevTest", 
											"CodedUI", 
											"Fakes", 
											"Simulator"
										  };

		private static readonly string[] KnownTestAttributes = new[] { "Test", "TestMethod", "Fact" };

		internal static bool IsKnownTestAttribute(this string text)
		{
			return KnownTestAttributes.Contains(text);
		}

		public static string GetMonth()
		{
			return string.Format("{0} {1}", DateTimeFormatInfo.CurrentInfo.GetMonthName(DateTime.Now.Month), DateTime.Now.Year);
		}

		public static bool TestCode(ProjectDefinition path)
		{
			var result = AllCode(path)
				&& (path.IsTest
					|| path.Source.EndsWith("test", StringComparison.OrdinalIgnoreCase)
					|| TestNames.Any(n => path.Source.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0));
			return result;
		}

		public static bool ProductionCode(ProjectDefinition path)
		{
			return !path.IsTest
				&& AllCode(path)
				&& !path.Source.EndsWith("test", StringComparison.OrdinalIgnoreCase)
				&& TestNames.All(n => path.Source.IndexOf(n, StringComparison.OrdinalIgnoreCase) == -1);
		}

		public static bool AllCode(ProjectDefinition path)
		{
			return path.Source.IndexOf("QuickStart", StringComparison.OrdinalIgnoreCase) == -1
				   && path.Source.IndexOf("Demo", StringComparison.OrdinalIgnoreCase) == -1;
		}
	}
}
