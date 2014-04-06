namespace ArchiMetrics.Analysis.Tests.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using NUnit.Framework;

	public sealed class MemberClassCouplingAnalyzerTests
	{
		private MemberClassCouplingAnalyzerTests()
		{
		}

		public class GivenAMemberClassCouplingAnalyzer
		{
			private MemberClassCouplingAnalyzer _analyzer;
			private Solution _solution;

			[SetUp]
			public void Setup()
			{
				_solution = CreateSolution(@"
namespace MyNamespace
{
	using System;
	public class MyClass
	{ 
		public int Number
		{ 
			get { return 1; }
		}

		public int WriteSomething()
		{
			Console.WriteLine(""blah"");
			var x = Number;
			return x;
		}
	}
}");
				_analyzer = new MemberClassCouplingAnalyzer(_solution.Projects.SelectMany(p => p.Documents).First().GetSemanticModelAsync().Result);
			}

			[Test]
			public void WhenCalculatingCouplingsThenIncludesPropertyDependencies()
			{
				var couplings = GetTypeCouplings();

				Assert.True(couplings.Any(x => x.UsedProperties.Any()));
			}

			[Test]
			public void WhenCalculatingCouplingsThenIncludesMemberDependencies()
			{
				var couplings = GetTypeCouplings();

				Assert.True(couplings.Any(x => x.UsedMethods.Any()));
			}

			private IEnumerable<ITypeCoupling> GetTypeCouplings()
			{
				var document = _solution.Projects.SelectMany(p => p.Documents).First();
				var method = document
					.GetSyntaxRootAsync()
					.Result
					.DescendantNodes()
					.OfType<SyntaxNode>()
					.First(n => n.IsKind(SyntaxKind.MethodDeclaration));

				var couplings = _analyzer.Calculate(method);
				return couplings;
			}

			private Solution CreateSolution(params string[] code)
			{
				using (var workspace = new CustomWorkspace())
				{
					workspace.AddSolution(
						SolutionInfo.Create(
							SolutionId.CreateNewId("Semantic"),
							VersionStamp.Default));
					var x = 1;
					var projectId = ProjectId.CreateNewId("testcode");
					var solution = workspace.CurrentSolution.AddProject(projectId, "testcode", "testcode.dll", LanguageNames.CSharp)
						.AddMetadataReference(projectId, new MetadataFileReference(typeof(object).Assembly.Location));
					solution = code.Aggregate(
						solution,
						(sol, c) => sol.AddDocument(DocumentId.CreateNewId(projectId), string.Format("TestClass{0}.cs", x++), c));

					return solution;
				}
			}
		}
	}
}
