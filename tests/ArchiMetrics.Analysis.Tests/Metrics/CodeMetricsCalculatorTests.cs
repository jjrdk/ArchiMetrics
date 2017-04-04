// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeMetricsCalculatorTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
    using System.Linq;
    using System.Threading.Tasks;
    using ArchiMetrics.Analysis.Metrics;
    using Common;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Xunit;

    public sealed class CodeMetricsCalculatorTests
    {
        private CodeMetricsCalculatorTests()
        {
        }

        public class GivenACodeMetricsCalculator
        {
            private readonly CodeMetricsCalculator _analyzer;

            public GivenACodeMetricsCalculator()
            {
                _analyzer = new CodeMetricsCalculator();
            }

            [Fact]
            public async Task CanCalculateMetricsForSnippet()
            {
                const string Snippet = @"namespace SomeNamespace
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

            [Fact]
            public void WhenCalculatingMetricsForNonCodeTextThenDoesNotThrow()
            {
                const string Text = "Hello World";

                var tree = CSharpSyntaxTree.ParseText(Text);

                var metrics = _analyzer.Calculate(new[] { tree }).Result;
                var result = metrics.AsArray();
            }

            [Fact]
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
                var task = await _analyzer.Calculate(new[] { tree }).ConfigureAwait(false);

                var metrics = task.AsArray();
                Assert.NotEmpty(metrics);
            }

            [Fact]
            public async Task CanCalculateMetricsForClassSnippet()
            {
                const string Snippet = @"
public class Something {
	public string Name { get; set; }
}
";
                var tree = CSharpSyntaxTree.ParseText(Snippet);
                var task = await _analyzer.Calculate(new[] { tree }).ConfigureAwait(false);

                var metrics = task.AsArray();
                Assert.NotEmpty(metrics);
            }

            [Fact]
            public async Task CanCalculateMetricsForCSharp6ClassSnippet()
            {
                const string Snippet = @"
public class Something {
    public string Name => ""Alberto"";
}
";
                var tree = CSharpSyntaxTree.ParseText(Snippet, CSharpParseOptions.Default);
                var task = await _analyzer.Calculate(new[] { tree }).ConfigureAwait(false);

                var metrics = task.AsArray();
                Assert.NotEmpty(metrics);
            }

            [Fact]
            public async Task CanCalculateMetricsForMethodSnippet()
            {
                const string Snippet = @"
public int Foo() { return 1; }
";
                var tree = CSharpSyntaxTree.ParseText(Snippet);
                var task = await _analyzer.Calculate(new[] { tree }).ConfigureAwait(false);

                var metrics = task.AsArray();
                Assert.NotEmpty(metrics);
            }

            [Fact]
            public async Task WhenClassDefinitionIsEmptyThenHasCyclomaticComplexityOfOne()
            {
                const string Text = @"namespace Testing
			{
				public class TestClass { }
			}";

                var metrics = await _analyzer.Calculate(CreateProject(Text), null);

                Assert.Equal(1, metrics.First().CyclomaticComplexity);
            }

            [Fact]
            public async Task WhenClassDefinitionHasEmptyConstructorThenHasCyclomaticComplexityOfOne()
            {
                const string Text = @"namespace Testing
			{
				public class TestClass {
	public TestClass(){}
}
			}";

                var metrics = await _analyzer.Calculate(CreateProject(Text), null);

                Assert.Equal(1, metrics.First().CyclomaticComplexity);
            }

            [Theory]
            [InlineData(@"namespace Testing
			{
				using System;
using System.Linq;
			}", 0)]
            [InlineData(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						const string x = ""blah"";
					}
				}
			}", 1)]
            [InlineData(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						{}
					}
				}
			}", 1)]
            [InlineData(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						var x = a + b;
					}
				}
			}", 2)]
            [InlineData(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						foreach(var a in x){}
					}
				}
			}", 2)]
            [InlineData(@"namespace Testing
			{
				public class TestClass {
					public TestClass()
			: base(SyntaxWalkerDepth.Node)
		{
		}

				}
			}", 1)]
            [InlineData(@"namespace Testing
			{
				public class TestClass {
					public TestClass()
		{
		}

				}
			}", 1)]
            [InlineData(@"namespace Testing
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
                var metrics = await _analyzer.Calculate(project, null).ConfigureAwait(false);

                Assert.Equal(loc, metrics.First().LinesOfCode);
            }

            private Project CreateProject(string text)
            {
                var workspace = new AdhocWorkspace();

                var projectId = ProjectId.CreateNewId("testcode");
                var solution = workspace.CurrentSolution.AddProject(projectId, "testcode", "testcode.dll", LanguageNames.CSharp);
                solution = solution.AddDocument(DocumentId.CreateNewId(projectId), "code.cs", text);
                return solution.Projects.First();
            }
        }

        public class GivenACodeMetricsCalculatorAndReadingDocumentation
        {
            private const string Snippet = @"namespace SomeNamespace
{
		/// <summary>
		/// Some class documentation.
		/// </summary>
		/// <typeparam name=""T"">Some type parameter</typeparam>
		/// <code>Some code</code>
		/// <example>Some example</example>
		/// <remarks>Some remark.</remarks>
	public class Something<T> where T : class,new()
	{
			/// <summary>
			/// Some member
			/// </summary>
			/// <param name=""x"">x parameter</param>
			/// <param name=""y"">y parameter</param>
			/// <typeparam name=""V"">Some type parameter</typeparam>
			/// <code>Some code</code>
			/// <example>Some example</example>
			/// <remarks>Some remark.</remarks>
		public void DoSomething<V>(string x, string y) where V : class, new()
		{
		}

		public string Name { get; set; }
	}
}
";

            private readonly CodeMetricsCalculator _analyzer;

            public GivenACodeMetricsCalculatorAndReadingDocumentation()
            {
                _analyzer = new CodeMetricsCalculator(new TypeDocumentationFactory(), new MemberDocumentationFactory());
            }

            [Fact]
            public async Task WhenTypeHasDocumentationThenReadsDocumentation()
            {
                var tree = CSharpSyntaxTree.ParseText(Snippet);
                var metrics = await _analyzer.Calculate(new[] { tree });

                Assert.NotNull(metrics);
            }
        }
    }
}
