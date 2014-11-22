// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoverageAnalyzerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CoverageAnalyzerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests
{
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using NUnit.Framework;

	public class CoverageAnalyzerTests : SolutionTestsBase
	{
		[Test]
		public async Task CanFindCoverage()
		{
			var code = @"namespace MyCode
{
	public class MyClass
	{
		public string Get(int value)
		{
			return GetInternal(value);
		}

		private string GetInternal(int value)
		{
			return value.ToString();
		}
	}
}";

			var test = @"namespace MyTest
{
	using MyCode;

	public class MyTestClass
	{
		[Test]
		public void MyTest()
		{
			var item = new MyClass();
			var x = item.Get(1);
		}
	}
}";

			var solution = CreateSolution(code, test);
			var projectCompilations = (from project in solution.Projects
									   let compilation = project.GetCompilationAsync()
									   select new
											  {
												  Documents = project.Documents.Select(
													  x => new
														   {
															   Tree = x.GetSyntaxTreeAsync(),
															   Root = x.GetSyntaxRootAsync()
														   }),
												  Compilation = compilation
											  })
				.ToArray();
			await Task.WhenAll(
				projectCompilations.SelectMany(x => x.Documents.SelectMany(y => new Task[] { y.Root, y.Tree })));

			var matches = (from x in projectCompilations
						   from doc in x.Documents
						   let model = x.Compilation.Result.GetSemanticModel(doc.Tree.Result)
						   let root = doc.Root.Result
						   from method in root.DescendantNodes()
							   .OfType<MethodDeclarationSyntax>()
						   where !method.AttributeLists.Any(
							   a => a.Attributes.Any(
								   b => b.Name.ToString()
											.IsKnownTestAttribute()))
						   select model.GetDeclaredSymbol(method))
				.ToArray();

			var analyzer = new CoverageAnalyzer(solution);
			var areReferencedTasks = matches.Select(analyzer.IsReferencedInTest).ToArray();
			var areReferenced = await Task.WhenAll(areReferencedTasks);

			Assert.IsTrue(areReferenced.All(x => x));
		}
		[Test]
		public async Task WhenNotCoveredByTestThenFindsNoCoverage()
		{
			var code = @"namespace MyCode
{
	public class MyClass
	{
		public string Get(int value)
		{
			return GetInternal(value);
		}

		private string GetInternal(int value)
		{
			return value.ToString();
		}
	}
}";

			var solution = CreateSolution(code);
			var projectCompilations = (from project in solution.Projects
									   let compilation = project.GetCompilationAsync()
									   select new
											  {
												  Documents = project.Documents.Select(
													  x => new
														   {
															   Tree = x.GetSyntaxTreeAsync(),
															   Root = x.GetSyntaxRootAsync()
														   }),
												  Compilation = compilation
											  })
				.ToArray();
			await Task.WhenAll(
				projectCompilations.SelectMany(x => x.Documents.SelectMany(y => new Task[] { y.Root, y.Tree })));

			var matches = (from x in projectCompilations
						   from doc in x.Documents
						   let model = x.Compilation.Result.GetSemanticModel(doc.Tree.Result)
						   let root = doc.Root.Result
						   from method in root.DescendantNodes()
							   .OfType<MethodDeclarationSyntax>()
						   where !method.AttributeLists.Any(
							   a => a.Attributes.Any(
								   b => b.Name.ToString()
											.IsKnownTestAttribute()))
						   select model.GetDeclaredSymbol(method))
				.ToArray();

			var analyzer = new CoverageAnalyzer(solution);
			var areReferencedTasks = matches.Select(analyzer.IsReferencedInTest).ToArray();
			var areReferenced = await Task.WhenAll(areReferencedTasks);

			Assert.IsFalse(areReferenced.All(x => x));
		}
	}
}
