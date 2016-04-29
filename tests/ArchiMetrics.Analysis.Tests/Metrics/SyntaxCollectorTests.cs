// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxCollectorTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxCollectorTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Metrics
{
    using System.Linq;
    using ArchiMetrics.Analysis.Metrics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public sealed class SyntaxCollectorTests
    {
        private SyntaxCollectorTests()
        {
        }

        public class GivenASyntaxCollector
        {
            private readonly SyntaxCollector _collector;

            public GivenASyntaxCollector()
            {
                _collector = new SyntaxCollector();
            }

            [Fact]
            public void WhenSnippetRootIsNamespaceThenOnlyFindsNamespace()
            {
                const string Snippet = @"namespace SomeNamespace
{
	public class Foo
	{
		public string Text { get; set; }
	}
}";
                var tree = CSharpSyntaxTree.ParseText(Snippet);
                var result = _collector.GetDeclarations(new[] { tree });


                Assert.All(result.MemberDeclarations.Cast<object>().Concat(result.Statements).Concat(result.NamespaceDeclarations).Concat(result.TypeDeclarations), x => Assert.IsAssignableFrom<NamespaceDeclarationSyntax>(x));
            }

            [Fact]
            public void WhenSnippetRootIsClassThenOnlyFindsType()
            {
                const string Snippet = @"public class Foo
{
	public string Text { get; set; }
}";
                var tree = CSharpSyntaxTree.ParseText(Snippet);
                var result = _collector.GetDeclarations(new[] { tree });

                Assert.NotEmpty(result.TypeDeclarations);
                Assert.Empty(result.MemberDeclarations.Cast<object>().Concat(result.Statements).Concat(result.NamespaceDeclarations));
            }

            [Fact]
            public void WhenSnippetRootIsStructThenOnlyFindsType()
            {
                const string Snippet = @"public struct Foo
{
	private string x = ""a"";

	public string Text { get{ return x; } }
}";
                var tree = CSharpSyntaxTree.ParseText(Snippet);
                var result = _collector.GetDeclarations(new[] { tree });

                Assert.All(result.MemberDeclarations.Cast<object>().Concat(result.Statements).Concat(result.NamespaceDeclarations).Concat(result.TypeDeclarations), x => Assert.IsAssignableFrom<TypeDeclarationSyntax>(x));
            }

            [Fact]
            public void WhenSnippetRootIsInterfaceThenOnlyFindsType()
            {
                const string Snippet = @"public interface Foo
{
	string Text { get; }
}";
                var tree = CSharpSyntaxTree.ParseText(Snippet);
                var result = _collector.GetDeclarations(new[] { tree });

                Assert.All(result.MemberDeclarations.Cast<object>().Concat(result.Statements).Concat(result.NamespaceDeclarations).Concat(result.TypeDeclarations), x => Assert.IsAssignableFrom<TypeDeclarationSyntax>(x));
            }

            [Fact]
            public void WhenSnippetRootIsPropertyThenOnlyFindsMember()
            {
                const string Snippet = @"public string Text { get; set; }";
                var tree = CSharpSyntaxTree.ParseText(Snippet);
                var result = _collector.GetDeclarations(new[] { tree });

                Assert.NotEmpty(result.MemberDeclarations);
                Assert.Empty(result.TypeDeclarations.Cast<object>().Concat(result.Statements).Concat(result.NamespaceDeclarations));
            }

            [Fact]
            public void WhenSnippetRootIsStatementThenOnlyFindsStatement()
            {
                const string Snippet = @"var x = 1;
var y = 2;";
                var tree = CSharpSyntaxTree.ParseText(Snippet, options: new CSharpParseOptions(LanguageVersion.CSharp6));
                var result = _collector.GetDeclarations(new[] { tree });

                Assert.NotEmpty(result.Statements);
                Assert.Empty(result.TypeDeclarations.Cast<object>().Concat(result.MemberDeclarations).Concat(result.NamespaceDeclarations));
            }
        }
    }
}
