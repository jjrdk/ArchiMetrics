namespace ArchiMetrics.Analysis.Tests.Metrics
{
	using System.Linq;

	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common.Metrics;

	using NUnit.Framework;

	using Roslyn.Compilers;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class MemberClassCouplingAnalyzerTests
	{
		private MemberClassCouplingAnalyzerTests()
		{
		}

		public class GivenAMemberClassCouplingAnalyzer
		{
			private MemberClassCouplingAnalyzer _analyzer;
			private ISolution _solution;

			[SetUp]
			public void Setup()
			{
				this._solution = this.CreateSolution(@"using System;
public class MyClass { 
	public int Number { 
		get { return 1; }
	}

	public void WriteSomething()
	{
		Console.WriteLine(""blah"");
		var x = this.Number;
	}
}");
				_analyzer = new MemberClassCouplingAnalyzer(_solution.Projects.SelectMany(p => p.Documents).First().GetSemanticModel());
			}

			[Test]
			public void WhenCalculatingCouplingsThenIncludesMemberDependencies()
			{
				var method = _solution.Projects
									  .SelectMany(p => p.Documents)
									  .First()
									  .GetSyntaxRoot()
									  .DescendantNodes()
									  .OfType<SyntaxNode>()
									  .First(n => n.Kind == SyntaxKind.MethodDeclaration);

				var couplings = _analyzer.Calculate(new MemberNode("blah", "blah", MemberKind.Method, 0, method));

				Assert.True(couplings.Any(x => x.UsedMethods.Length > 0));
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
}
