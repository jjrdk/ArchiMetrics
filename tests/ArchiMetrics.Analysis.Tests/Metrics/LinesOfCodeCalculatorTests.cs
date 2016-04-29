// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinesOfCodeCalculatorTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LinesOfCodeCalculatorTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Metrics
{
	using System.Linq;
	using ArchiMetrics.Analysis.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Xunit;

    public sealed class LinesOfCodeCalculatorTests
	{
		private LinesOfCodeCalculatorTests()
		{
		}

		public class GivenAStatementsAnalyzer
		{
			private readonly LinesOfCodeCalculator _analyzer;
            
			public GivenAStatementsAnalyzer()
			{
				_analyzer = new LinesOfCodeCalculator();
			}

			[Fact]
			public void WhenOnlyAssigningConstThenHasZeroLinesOfCode()
			{
				const string Text = @"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						const string x = ""blah"";
					}
				}
			}";

				var syntaxTree = CSharpSyntaxTree.ParseText(Text);
				var root = syntaxTree
					.GetRoot()
					.DescendantNodes()
					.First(c => c.IsKind(SyntaxKind.MethodDeclaration));

				var loc = _analyzer.Calculate(root);

				Assert.Equal(0, loc);
			}

            [Theory]
			[InlineData(@"public void SomeMethod() { const string x = ""blah""; }", 0, SyntaxKind.MethodDeclaration)]
			[InlineData(@"public TestClass() { }", 1, SyntaxKind.ConstructorDeclaration)]
			[InlineData(@"public int GetValue() { return 1; }", 1, SyntaxKind.MethodDeclaration)]
			[InlineData(@"public double GetValue(double x)
		{
			if (x % 2 == 0.0)
			{
				return x;
			}
			return x + 1;
		}", 3, SyntaxKind.MethodDeclaration)]
			public void WhenCalculatingForMemberNodeHasExpectedLinesOfCode(string code, int expected, SyntaxKind kind)
			{
				var text = string.Format(
@"namespace Testing
			{{
				public class TestClass {{
					{0}
				}}
			}}", 
			   code);

				var syntaxTree = CSharpSyntaxTree.ParseText(text);
				var root = syntaxTree
					.GetRoot()
					.DescendantNodes()
					.First(c => c.IsKind(kind));
				var loc = _analyzer.Calculate(root);

				Assert.Equal(expected, loc);
			}

            [Theory]
			[InlineData(@"public void SomeMethod() {
						const string x = ""blah"";
					}", 0)]
			[InlineData(@"public TestClass() { }", 1)]
			[InlineData(@"public int Value { get; set; }", 2)]
			[InlineData(@"public int Value { get; }", 1)]
			[InlineData(@"public int Value { set; }", 1)]
			[InlineData(@"public int GetValue() { return 1; }", 1)]
			[InlineData(@"public double GetValue(double x)
		{
			if (x % 2 == 0.0)
			{
				return x;
			}
			return x + 1;
		}", 3)]
			public void WhenCountingLinesOfCodeThenHasExpectedCount(string code, int count)
			{
				var text = string.Format(
@"namespace Testing
			{{
				public class TestClass {{
					{0}
				}}
			}}", 
			   code);

				var syntaxTree = CSharpSyntaxTree.ParseText(text);
				var root = syntaxTree.GetRoot();
				var loc = _analyzer.Calculate(root);

				Assert.Equal(count, loc);
			}
		}
	}
}
