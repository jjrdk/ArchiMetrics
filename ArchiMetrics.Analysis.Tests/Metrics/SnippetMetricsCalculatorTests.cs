// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SnippetMetricsCalculatorTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SnippetMetricsCalculatorTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Metrics
{
	using Analysis.Metrics;
	using NUnit.Framework;

	public class SnippetMetricsCalculatorTests
	{
		[TestCase("var x = 1;")]
		[TestCase("var t = Task.FromResult(new object());")]
		public void CanCompile(string snippet)
		{
			var calculator = new SnippetMetricsCalculator();
			var result = calculator.Calculate(snippet);

			Assert.NotNull(result);
		}
	}
}
