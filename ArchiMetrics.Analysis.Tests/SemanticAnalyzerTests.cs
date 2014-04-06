// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticAnalyzerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SemanticAnalyzerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests
{
	using System.Linq;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using NUnit.Framework;

	public class SemanticAnalyzerTests : SolutionTestsBase
	{
		[Test]
		public async void CanFindUnusedParameters()
		{
			const string Code = @"namespace abc
{
	using System;

	public class MyClass
	{
		public int Foo(int x, string y)
		{
			return x;
		}
	}
}";
			var solution = CreateSolution(Code);
			var doc = solution.Projects.First().Documents.First();

			var model = await doc.GetSemanticModelAsync();
			var root = await doc.GetSyntaxRootAsync();
			var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
			var analyzer = new SemanticAnalyzer(model);
			var unused = analyzer.GetUnusedParameters(method);

			Assert.True(unused.Any(x => x.Identifier.ValueText == "y"));
		}

		[Test]
		public async void CanFindPossibleStaticMethod()
		{
			const string Code = @"namespace abc
{
	using System;

	public class MyClass
	{
		public int Foo()
		{
			return 5;
		}
	}
}";
			var solution = CreateSolution(Code);
			var doc = solution.Projects.First().Documents.First();

			var model = await doc.GetSemanticModelAsync();
			var root = await doc.GetSyntaxRootAsync();
			var type = root.DescendantNodes().OfType<TypeDeclarationSyntax>().First();
			var analyzer = new SemanticAnalyzer(model);
			var staticMethods = analyzer.GetPossibleStaticMethods(type);

			CollectionAssert.IsNotEmpty(staticMethods);
		}

		[Test]
		public async void DoesNotSuggestAlreadyStaticMethods()
		{
			const string Code = @"namespace abc
{
	using System;

	public class MyClass
	{
		public static int Foo()
		{
			return 5;
		}
	}
}";
			var solution = CreateSolution(Code);
			var doc = solution.Projects.First().Documents.First();

			var model = await doc.GetSemanticModelAsync();
			var root = await doc.GetSyntaxRootAsync();
			var type = root.DescendantNodes().OfType<TypeDeclarationSyntax>().First();
			var analyzer = new SemanticAnalyzer(model);
			var staticMethods = analyzer.GetPossibleStaticMethods(type);

			CollectionAssert.IsEmpty(staticMethods);
		}
	}
}