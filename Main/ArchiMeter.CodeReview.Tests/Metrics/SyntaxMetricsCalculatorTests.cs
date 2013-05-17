// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxMetricsCalculatorTests.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxMetricsCalculatorTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.CodeReview.Tests
{
	using System.Linq;
	using CodeReview.Metrics;
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;

	public class SyntaxMetricsCalculatorTests
	{
		[Test]
		public void CanGetHalsteadMetricsForClassSnippet()
		{
			var code = @"public class Something { 
public int Number { get{ return a - b; } }
public string GetValue() { return ""x"" + a; } 
}";
			var root = SyntaxTree.ParseText(code).GetRoot();

			var metrics = new SyntaxMetricsCalculator().Calculate(root);

			Assert.NotNull(metrics);
		}

		[Test]
		public void CanGetHalsteadMetricsForMethodSnippet()
		{
			var code = @"public string GetValue() { return ""x""; }";
			var root = SyntaxTree.ParseText(code).GetRoot();

			var metrics = new SyntaxMetricsCalculator().Calculate(root);

			Assert.NotNull(metrics);
		}

		[Test]
		public void CanGetHalsteadMetricsForArbitrarySnippet()
		{
			var code = @"return ""x"";";
			var root = SyntaxTree.ParseText(code).GetRoot();

			var metrics = new SyntaxMetricsCalculator().Calculate(root);

			Assert.NotNull(metrics);
		}

		[Test]
		public void CanGetHalsteadMetricsForMultipleArbitrarySnippets()
		{
			var code = @"var a = ""x""; return a;";
			var root = SyntaxTree.ParseText(code).GetRoot();

			var metrics = new SyntaxMetricsCalculator().Calculate(root);

			Assert.NotNull(metrics);
		}

		[TestCase(@"public class Something { 
public int Number { get{ return a - b; } }
public string GetValue() { return ""x"" + a; } 
}")]
		[TestCase(@"return ""x"";")]
		[TestCase(@"var a = ""x""; return a;")]
		public void CodeHasEffort(string code)
		{
			var metrics = new SyntaxMetricsCalculator().Calculate(code);

			var effort = metrics.Sum(m => m.GetEffort().TotalSeconds * 18);
			Assert.Greater(effort, 0);
		}
	}
}