namespace ArchiMetrics.Analysis.Tests
{
	using System.Linq;
	using NUnit.Framework;
	using Roslyn.Compilers;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class SemanticAnalyzerTests
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
			DocumentId did;
			ProjectId pid;
			var solution = Solution.Create(SolutionId.CreateNewId("test"))
				.AddCSharpProject("x", "x", out pid)
				.AddDocument(pid, "x.cs", Code, out did)
				.AddMetadataReferences(pid, new[] { new MetadataFileReference(typeof(object).Assembly.Location) });
			var doc = solution.GetDocument(did);

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
			DocumentId did;
			ProjectId pid;
			var solution = Solution.Create(SolutionId.CreateNewId("test"))
				.AddCSharpProject("x", "x", out pid)
				.AddDocument(pid, "x.cs", Code, out did)
				.AddMetadataReferences(pid, new[] { new MetadataFileReference(typeof(object).Assembly.Location) });
			var doc = solution.GetDocument(did);

			var model = await doc.GetSemanticModelAsync();
			var root = await doc.GetSyntaxRootAsync();
			var type = root.DescendantNodes().OfType<TypeDeclarationSyntax>().First();
			var analyzer = new SemanticAnalyzer(model);
			var staticMethods = analyzer.GetPossibleStaticMethods(type);

			CollectionAssert.IsNotEmpty(staticMethods);
		}
	}
}