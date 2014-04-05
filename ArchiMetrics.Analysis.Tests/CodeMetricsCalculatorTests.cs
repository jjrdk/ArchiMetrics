// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeMetricsCalculatorTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeMetricsCalculatorTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests
{
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using NUnit.Framework;
	

	public sealed class CodeMetricsCalculatorTests
	{
		private CodeMetricsCalculatorTests()
		{
		}

		public class GivenACodeMetricsCalculator
		{
			private CodeMetricsCalculator _calculator;

			[SetUp]
			public void Setup()
			{
				_calculator = new CodeMetricsCalculator();
			}

			[Test]
			public void WhenCalculatingMetricsForCodeSnippetThenReturnsMetrics()
			{
				var snippet = @"public int GetValue(int x)
{
	if(x% 2 == 0)
	{
		return x-2;
	}

	return x;
}";
				var tree = CSharpSyntaxTree.ParseText(snippet);
				var metrics = _calculator.Calculate(new[] { tree });

				Assert.NotNull(metrics);
			}
		}
	}
}
