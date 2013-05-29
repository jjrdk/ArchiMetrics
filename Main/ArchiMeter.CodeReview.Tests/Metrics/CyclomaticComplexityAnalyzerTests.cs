// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CyclomaticComplexityAnalyzerTests.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CyclomaticComplexityAnalyzerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Tests.Metrics
{
	using System.Linq;
	using CodeReview.Metrics;
	using Common.Metrics;
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;

	public sealed class CyclomaticComplexityAnalyzerTests
	{
		private CyclomaticComplexityAnalyzerTests()
		{
		}

		public class GivenACyclomaticComplexityAnalyzer
		{
			private CyclomaticComplexityAnalyzer _analyzer;

			[SetUp]
			public void SetUp()
			{
				_analyzer = new CyclomaticComplexityAnalyzer();
			}

			[TestCase("public abstract void DoSomething();", 1)]
			[TestCase("void DoSomething();", 1)]
			[TestCase("void DoSomething(){ var x = a && b; }", 2)]
			[TestCase(@"public void DoSomething(){
	try
	{
		var x = 1 + 2;
		var y = x + 2;
	}
	catch
	{
		throw new Exception();
	}
}", 2)]
			[TestCase(@"public void DoSomething(){
	try
	{
		var x = 1 + 2;
		var y = x + 2;
	}
	catch(ArgumentNullException ane)
	{
		throw new Exception();
	}
	catch(OutOfRangeException ane)
	{
		throw new Exception();
	}
}", 3)]
			[TestCase(@"public void DoSomething(){
	if(x == 1)
	{
		var y = x + 2;
	}
	else
	{		
		var y = 1 + 2;
	}
}", 2)]
			[TestCase(@"public int DoSomething(){
	switch(x){
		case ""a"": return 1;
		case ""b"": return 2;
		default: return 0;
	}
}", 3)]
			[TestCase(@"public int DoSomething(){
	var x = a > 2 ? 1 : 0;
	}
}", 2)]
			[TestCase(@"public int DoSomething(){
	var x = a ?? new object();
	}
}", 2)]
			[TestCase(@"public int DoSomething(){
		var numbers = new[] { 1, 2, 3 };
		var odds = numbers.Where(n => n != 1).ToArray();
	}
}", 1)]
			public void MethodHasExpectedComplexity(string method, int expectedComplexity)
			{
				var syntaxNode = SyntaxTree.ParseText(method)
					.GetRoot()
					.DescendantNodes()
					.OfType<MethodDeclarationSyntax>()
					.First();
				var node = new MemberNode(string.Empty, "test", MemberKind.Method, 0, syntaxNode);
				var result = _analyzer.Calculate(node);

				Assert.AreEqual(expectedComplexity, result);
			}
		}
	}
}
