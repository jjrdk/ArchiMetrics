// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticAnalyzerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	using System.Threading.Tasks;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Xunit;

    public class SemanticAnalyzerTests : SolutionTestsBase
	{
		[Fact]
		public async Task CanFindUnusedParameters()
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

		[Fact]
		public async Task CanFindPossibleStaticMethod()
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

			Assert.NotEmpty(staticMethods);
		}

		[Fact]
		public async Task DoesNotSuggestAlreadyStaticMethods()
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

			Assert.Empty(staticMethods);
		}
	}
}