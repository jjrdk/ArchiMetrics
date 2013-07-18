namespace ArchiMetrics.CodeReview.Tests.Rules
{
	using System;
	using System.Linq;
	using NUnit.Framework;

	using Roslyn.Compilers;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;
	using Semantic;

	public class SemanticRulesTests
	{
		[TestCase(@"public class MyClass { public int GetNumber() { return 1; } }", SyntaxKind.MethodDeclaration, typeof(UnusedMethodRule))]
		[TestCase(@"public class MyClass { public int Number { get { return 1; } } }", SyntaxKind.GetAccessorDeclaration, typeof(UnusedGetPropertyRule))]
		[TestCase(@"public class MyClass { private int _number = 0; public int Number { get { return _number; } set { _number = value; } } }", SyntaxKind.GetAccessorDeclaration, typeof(UnusedSetPropertyRule))]
		public void CanFindUnusedCode(string code, SyntaxKind kind, Type semanticRule)
		{
			var solution = CreateSolution(code);
			var method = solution.Projects.SelectMany(p => p.Documents)
				.SelectMany(d =>
							{
								var semanticModel = d.GetSemanticModel();
								return d.GetSyntaxRoot()
									.DescendantNodes()
									.OfType<SyntaxNode>()
									.Select(r =>
										new
										{
											node = r,
											model = semanticModel
										});
							})
							.First(x => x.node.Kind == kind);

			var rule = (ISemanticEvaluation)Activator.CreateInstance(semanticRule);
			var result = rule.Evaluate(method.node, method.model, solution);

			Assert.NotNull(result);
		}

		private ISolution CreateSolution(params string[] code)
		{
			int x = 1;
			ProjectId pid;
			DocumentId did;
			var solution = code.Aggregate(
				Solution.Create(SolutionId.CreateNewId("Semantic"))
					.AddCSharpProject("testcode.dll", "testcode", out pid),
				(sol, c) => sol.AddDocument(pid, string.Format("TestClass{0}.cs", x++), c, out did))
				.AddProjectReferences(pid, new ProjectId[0])
				.AddMetadataReference(pid, new MetadataFileReference(typeof(object).Assembly.Location));

			return solution;
		}
	}
}
