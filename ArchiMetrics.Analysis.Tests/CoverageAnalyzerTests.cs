namespace ArchiMetrics.Analysis.Tests
{
	using System.Linq;
	using ArchiMetrics.Common;
	using NUnit.Framework;
	using Roslyn.Compilers;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

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

	public abstract class SolutionTestsBase
	{
		protected ISolution CreateSolution(params string[] code)
		{
			var x = 1;
			ProjectId pid;
			DocumentId did;
			var solution = code.Aggregate(
				Solution.Create(SolutionId.CreateNewId("Analysis"))
					.AddCSharpProject("testcode.dll", "testcode", out pid),
				(sol, c) => sol.AddDocument(pid, string.Format("TestClass{0}.cs", x++), c, out did))
				.AddProjectReferences(pid, new ProjectId[0])
				.AddMetadataReference(pid, new MetadataFileReference(typeof(object).Assembly.Location));

			return solution;
		}
	}
}
