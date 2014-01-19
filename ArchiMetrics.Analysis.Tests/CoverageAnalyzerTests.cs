// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoverageAnalyzerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using ArchiMetrics.Common;
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;

	public class CoverageAnalyzerTests : SolutionTestsBase
	{
		[Test]
		public void CanFindCoverage()
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
			var matches = (from project in solution.Projects
						   let compilation = project.GetCompilation()
						   from doc in project.Documents
						   let model = compilation.GetSemanticModel(doc.GetSyntaxTree())
						   let root = doc.GetSyntaxRoot()
						   from method in root.DescendantNodes().OfType<MethodDeclarationSyntax>()
						   where !method.AttributeLists.Any(a => a.Attributes.Any(b => b.Name.ToString().IsKnownTestAttribute()))
						   select model.GetDeclaredSymbol(method))
				.ToArray();

			var analyzer = new CoverageAnalyzer(solution);
			var areReferenced = matches.Select(analyzer.IsReferencedInTest).ToArray();

			Assert.IsTrue(areReferenced.All(x => x));
		}
	}
}
