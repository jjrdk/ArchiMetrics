// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberClassCouplingAnalyzerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberClassCouplingAnalyzerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common.Metrics;
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
				_analyzer = new MemberClassCouplingAnalyzer(_solution.Projects.SelectMany(p => p.Documents).First().GetSemanticModel());
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
					.GetSyntaxRoot()
					.DescendantNodes()
					.OfType<SyntaxNode>()
					.First(n => n.Kind == SyntaxKind.MethodDeclaration);

				var couplings = _analyzer.Calculate(method);
				return couplings;
			}

			private Solution CreateSolution(params string[] code)
			{
				var x = 1;
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
