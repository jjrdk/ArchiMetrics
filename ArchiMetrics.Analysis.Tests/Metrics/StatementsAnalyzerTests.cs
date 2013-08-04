// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatementsAnalyzerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StatementsAnalyzerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Metrics
{
	using System.Linq;
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common.Metrics;
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;

	public sealed class StatementsAnalyzerTests
	{
		private StatementsAnalyzerTests()
		{
		}

		public class GivenAStatementsAnalyzer
		{
			private StatementsAnalyzer _analyzer;

			[SetUp]
			public void Setup()
			{
				_analyzer = new StatementsAnalyzer();
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

				var root = SyntaxTree.ParseText(Text)
											.GetRoot()
											.DescendantNodes()
											.First(c => c.Kind == SyntaxKind.MethodDeclaration);
				var node = new MemberNode("a", "b", MemberKind.Method, 0, root);
				var loc = _analyzer.Calculate(node);

				Assert.AreEqual(0, loc);
			}
		}
	}
}
