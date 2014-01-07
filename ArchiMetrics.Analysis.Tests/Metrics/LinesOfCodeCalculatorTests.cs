// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinesOfCodeCalculatorTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using ArchiMetrics.Common.Metrics;
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;

	public sealed class LinesOfCodeCalculatorTests
	{
		private LinesOfCodeCalculatorTests()
		{
		}

		public class GivenAStatementsAnalyzer
		{
			private LinesOfCodeCalculator _analyzer;

			[SetUp]
			public void Setup()
			{
				_analyzer = new LinesOfCodeCalculator();
			}

			[Test]
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

				var syntaxTree = SyntaxTree.ParseText(Text);
				var compilation = Compilation.Create("x", syntaxTrees: new[] { syntaxTree });
				var model = compilation.GetSemanticModel(syntaxTree);
				var root = syntaxTree
					.GetRoot()
					.DescendantNodes()
					.First(c => c.Kind == SyntaxKind.MethodDeclaration);
				var node = new MemberNode("a", "b", MemberKind.Method, 0, root, model);
				var loc = _analyzer.Calculate(node);

				Assert.AreEqual(0, loc);
			}

			[TestCase(@"public void SomeMethod() { const string x = ""blah""; }", 0, SyntaxKind.MethodDeclaration, MemberKind.Method)]
			[TestCase(@"public TestClass() { }", 1, SyntaxKind.ConstructorDeclaration, MemberKind.Constructor)]
			[TestCase(@"public int GetValue() { return 1; }", 1, SyntaxKind.MethodDeclaration, MemberKind.Method)]
			[TestCase(@"public double GetValue(double x)
		{
			if (x % 2 == 0.0)
			{
				return x;
			}
			return x + 1;
		}", 3, SyntaxKind.MethodDeclaration, MemberKind.Method)]
			public void WhenCalculatingForMemberNodeHasExpectedLinesOfCode(string code, int expected, SyntaxKind kind, MemberKind memberKind)
			{
				var text = string.Format(
@"namespace Testing
			{{
				public class TestClass {{
					{0}
				}}
			}}",
			   code);

				var syntaxTree = SyntaxTree.ParseText(text);
				var compilation = Compilation.Create("x", syntaxTrees: new[] { syntaxTree });
				var model = compilation.GetSemanticModel(syntaxTree);
				var root = syntaxTree
											.GetRoot()
											.DescendantNodes()
											.First(c => c.Kind == kind);
				var node = new MemberNode("a", "b", memberKind, 0, root, model);
				var loc = _analyzer.Calculate(node);

				Assert.AreEqual(expected, loc);
			}

			[TestCase(@"public void SomeMethod() {
						const string x = ""blah"";
					}", 0)]
			[TestCase(@"public TestClass() { }", 1)]
			[TestCase(@"public int Value { get; set; }", 2)]
			[TestCase(@"public int Value { get; }", 1)]
			[TestCase(@"public int Value { set; }", 1)]
			[TestCase(@"public int GetValue() { return 1; }", 1)]
			[TestCase(@"public double GetValue(double x)
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

				var syntaxTree = SyntaxTree.ParseText(text);
				var root = syntaxTree.GetRoot();
				var loc = _analyzer.Calculate(root);

				Assert.AreEqual(count, loc);
			}
		}
	}
}
