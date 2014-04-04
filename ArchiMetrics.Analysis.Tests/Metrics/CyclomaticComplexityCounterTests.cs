// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CyclomaticComplexityCounterTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CyclomaticComplexityAnalyzerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Metrics
{
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using NUnit.Framework;
	
	

	public sealed class CyclomaticComplexityCounterTests
	{
		private CyclomaticComplexityCounterTests()
		{
		}

		public class GivenACyclomaticComplexityAnalyzer
		{
			private CyclomaticComplexityCounter _counter;

			[SetUp]
			public void SetUp()
			{
				_counter = new CyclomaticComplexityCounter();
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
		var n = numbers.Where(n => n != 1).ToArray();
	}
}", 1)]
			[TestCase(@"public int DoSomething(){
		var numbers = new[] { 1, 2, 3 };
		var odds = numbers.Where(n => { if(n != 1) { return n %2 == 0; } else { return false; } }).ToArray();
	}
}", 3)]
			[TestCase(@"
namespace MyNs
{
	using System;
	using System.Threading.Tasks;

	public class MyClass
	{
		public void DoSomething()
		{
				var task = Task.Factory.StartNew(() => { Console.WriteLine(""blah""); });
		}
	}
}", 1)]
			public void MethodHasExpectedComplexity(string method, int expectedComplexity)
			{
				var tree = CSharpSyntaxTree.ParseText(method);
				var compilation = CSharpCompilation.Create(
					"x",
					syntaxTrees: new[] { tree },
					references: new[] { new MetadataFileReference(typeof(object).Assembly.Location), new MetadataFileReference(typeof(Task).Assembly.Location) },
					options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, usings: new[] { "System", "System.Threading.Tasks" }));

				var model = compilation.GetSemanticModel(tree);
				var syntaxNode = tree
					.GetRoot()
					.DescendantNodes()
					.OfType<MethodDeclarationSyntax>()
					.First();

				var result = _counter.Calculate(syntaxNode, model);

				Assert.AreEqual(expectedComplexity, result);
			}

			[TestCase(@"namespace MyNs
{
	public class MyClass
	{
		private EventHandler _innerHandler;

		public event EventHandler MyEvent
		{
			add { _innerHandler += value; }
			remove { _innerHandler -= value; }
		}
	}
}", 1)]
			public void EventAddAccessorHasExpectedComplexity(string code, int expectedComplexity)
			{
				var tree = CSharpSyntaxTree.ParseText(code);
				var compilation = Compilation.Create(
					"x",
					syntaxTrees: new[] { tree },
					references: new[] { new MetadataFileReference(typeof(object).Assembly.Location), new MetadataFileReference(typeof(Task).Assembly.Location) },
					options: new CompilationOptions(OutputKind.DynamicallyLinkedLibrary, usings: new[] { "System", "System.Threading.Tasks" }));

				var model = compilation.GetSemanticModel(tree);
				var syntaxNode = tree
					.GetRoot()
					.DescendantNodes()
					.OfType<AccessorDeclarationSyntax>()
					.First();

				var result = _counter.Calculate(syntaxNode, model);

				Assert.AreEqual(expectedComplexity, result);
			}
		}
	}
}
