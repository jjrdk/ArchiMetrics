// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeMetricsCalculatorTests.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeMetricsCalculatorTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Metrics
{
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.MSBuild;
	using NUnit.Framework;

	public sealed class CodeMetricsCalculatorTests
	{
		private CodeMetricsCalculatorTests()
		{
		}

		public class GivenACodeMetricsCalculator
		{
			private CodeMetricsCalculator _analyzer;

			[SetUp]
			public void Setup()
			{
				_analyzer = new CodeMetricsCalculator();
			}

			[Test]
			public async Task CanCalculateMetricsForSnippet()
			{
				const string Snippet = @"
namespace SomeNamespace
{
	public class Something {
		publis string Name { get; set; }
	}
}
";
				var tree = CSharpSyntaxTree.ParseText(Snippet);
				var metrics = await _analyzer.Calculate(new[] { tree });

				Assert.NotNull(metrics);
			}

			[Test]
			public void WhenCalculatingMetricsForNonCodeTextThenDoesNotThrow()
			{
				Assert.DoesNotThrow(() =>
									{
										const string Text = "Hello World";

										var tree = CSharpSyntaxTree.ParseText(Text);

										var metrics = _analyzer.Calculate(new[] { tree }).Result;
										var result = metrics.AsArray();
									});
			}

			[Test]
			public async Task CanCalculateMetricsForNamespaceSnippet()
			{
				const string Snippet = @"
namespace SomeNamespace
{
	public class Something {
		publis string Name { get; set; }
	}
}
";
				var tree = CSharpSyntaxTree.ParseText(Snippet);
				var task = await _analyzer.Calculate(new[] { tree });

				var metrics = task.AsArray();
				Assert.IsNotEmpty(metrics);
			}

			[Test]
			public async Task CanCalculateMetricsForClassSnippet()
			{
				const string Snippet = @"
public class Something {
	public string Name { get; set; }
}
";
				var tree = CSharpSyntaxTree.ParseText(Snippet);
				var task = await _analyzer.Calculate(new[] { tree });

				var metrics = task.AsArray();
				Assert.IsNotEmpty(metrics);
			}

			[Test]
			public async Task CanCalculateMetricsForMethodSnippet()
			{
				const string Snippet = @"
public int Foo() { return 1; }
";
				var tree = CSharpSyntaxTree.ParseText(Snippet);
				var task = await _analyzer.Calculate(new[] { tree });

				var metrics = task.AsArray();
				Assert.IsNotEmpty(metrics);
			}
		
			[Test]
			public async Task WhenClassDefinitionIsEmptyThenHasCyclomaticComplexityOfOne()
			{
				const string Text = @"namespace Testing
			{
				public class TestClass { }
			}";

				var metrics = await _analyzer.Calculate(CreateProject(Text), null);

				Assert.AreEqual(1, metrics.First().CyclomaticComplexity);
			}

			[Test]
			public async Task WhenClassDefinitionHasEmptyConstructorThenHasCyclomaticComplexityOfOne()
			{
				const string Text = @"namespace Testing
			{
				public class TestClass {
	public TestClass(){}
}
			}";

				var metrics = await _analyzer.Calculate(CreateProject(Text), null);
				
				Assert.AreEqual(1, metrics.First().CyclomaticComplexity);
			}

			[TestCase(@"namespace Testing
			{
				using System;
using System.Linq;
			}", 0)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						const string x = ""blah"";
					}
				}
			}", 1)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						{}
					}
				}
			}", 1)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						var x = a + b;
					}
				}
			}", 2)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						foreach(var a in x){}
					}
				}
			}", 2)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public TestClass()
			: base(SyntaxWalkerDepth.Node)
		{
		}

				}
			}", 1)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public TestClass()
		{
		}

				}
			}", 1)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod(SyntaxNode node)
					{
						base.VisitBinaryExpression(node);
						switch (node.Kind)
						{
							case SyntaxKind.LogicalNotExpression:
								_counter++;
								break;
							case SyntaxKind.ExclusiveOrExpression:
								break;
						}
					}
				}
			}", 4)]
			public async Task CodeHasExpectedLinesOfCode(string code, int loc)
			{
				var project = CreateProject(code);
				var metrics = await _analyzer.Calculate(project, null);

				Assert.AreEqual(loc, metrics.First().LinesOfCode);
			}

			[TestCase(@"namespace Testing
			{
				using System;
using System.Linq;
			}", 3)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						const string x = ""blah"";
					}
				}
			}", 8)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						{}
					}
				}
			}", 8)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						var x = a + b;
					}
				}
			}", 8)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						foreach(var a in x){}
					}
				}
			}", 8)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public TestClass()
			: base(SyntaxWalkerDepth.Node)
		{
		}

				}
			}", 10)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public TestClass()
		{
		}

				}
			}", 9)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod(SyntaxNode node)
					{
						base.VisitBinaryExpression(node);
						switch (node.Kind)
						{
							case SyntaxKind.LogicalNotExpression:
								_counter++;
								break;
							case SyntaxKind.ExclusiveOrExpression:
								break;
						}
					}
				}
			}", 17)]
			public async Task CodeHasExpectedSourceLinesOfCode(string code, int loc)
			{
				var project = CreateProject(code);
				var metrics = await _analyzer.Calculate(project, null);

				Assert.AreEqual(loc, metrics.First().SourceLinesOfCode);
			}

			private Project CreateProject(string text)
			{
				var workspace = new CustomWorkspace();
				workspace.AddSolution(
					SolutionInfo.Create(
						SolutionId.CreateNewId("test"), 
						VersionStamp.Create()));
				var projectId = ProjectId.CreateNewId("testcode");
				var solution = workspace.CurrentSolution.AddProject(
					projectId, 
					"testcode", 
					"testcode.dll", 
					LanguageNames.CSharp);
				solution = solution.AddDocument(DocumentId.CreateNewId(projectId), "code.cs", text);
				return solution.Projects.First();
			}
		}
	}
}
