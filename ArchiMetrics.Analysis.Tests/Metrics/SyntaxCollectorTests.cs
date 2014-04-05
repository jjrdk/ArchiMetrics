// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxCollectorTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using System.Threading;
	using ArchiMetrics.Analysis.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using NUnit.Framework;
	

	public sealed class SyntaxCollectorTests
	{
		private SyntaxCollectorTests()
		{
		}

		public class GivenASyntaxCollector
		{
			private SyntaxCollector _collector;

			[SetUp]
			public void Setup()
			{
				_collector = new SyntaxCollector();
			}

			[Test]
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

				CollectionAssert.AllItemsAreInstancesOfType(result.MemberDeclarations.Cast<object>().Concat(result.Statements).Concat(result.NamespaceDeclarations).Concat(result.TypeDeclarations), typeof(NamespaceDeclarationSyntax));
			}

			[Test]
			public void WhenSnippetRootIsClassThenOnlyFindsType()
			{
				const string Snippet = @"public class Foo
{
	public string Text { get; set; }
}";
				var tree = CSharpSyntaxTree.ParseText(Snippet);
				var result = _collector.GetDeclarations(new[] { tree });

				Assert.IsNotEmpty(result.TypeDeclarations);
				Assert.IsEmpty(result.MemberDeclarations.Cast<object>().Concat(result.Statements).Concat(result.NamespaceDeclarations));
			}

			[Test]
			public void WhenSnippetRootIsStructThenOnlyFindsType()
			{
				const string Snippet = @"public struct Foo
{
	private string x = ""a"";

	public string Text { get{ return x; } }
}";
				var tree = CSharpSyntaxTree.ParseText(Snippet);
				var result = _collector.GetDeclarations(new[] { tree });

				CollectionAssert.AllItemsAreInstancesOfType(result.MemberDeclarations.Cast<object>().Concat(result.Statements).Concat(result.NamespaceDeclarations).Concat(result.TypeDeclarations), typeof(TypeDeclarationSyntax));
			}

			[Test]
			public void WhenSnippetRootIsInterfaceThenOnlyFindsType()
			{
				const string Snippet = @"public interface Foo
{
	string Text { get; }
}";
				var tree = CSharpSyntaxTree.ParseText(Snippet);
				var result = _collector.GetDeclarations(new[] { tree });

				CollectionAssert.AllItemsAreInstancesOfType(result.MemberDeclarations.Cast<object>().Concat(result.Statements).Concat(result.NamespaceDeclarations).Concat(result.TypeDeclarations), typeof(TypeDeclarationSyntax));
			}

			[Test]
			public void WhenSnippetRootIsPropertyThenOnlyFindsMember()
			{
				const string Snippet = @"public string Text { get; set; }";
				var tree = CSharpSyntaxTree.ParseText(Snippet);
				var result = _collector.GetDeclarations(new[] { tree });

				Assert.IsNotEmpty(result.MemberDeclarations);
				Assert.IsEmpty(result.TypeDeclarations.Cast<object>().Concat(result.Statements).Concat(result.NamespaceDeclarations));
			}

			[Test]
			public void WhenSnippetRootIsStatementThenOnlyFindsStatement()
			{
				const string Snippet = @"var x = 1;
var y = 2;";
				var tree = CSharpSyntaxTree.ParseText(Snippet, options: new CSharpParseOptions(LanguageVersion.CSharp4, kind: SourceCodeKind.Interactive));
				var result = _collector.GetDeclarations(new[] { tree });

				Assert.IsNotEmpty(result.Statements);
				Assert.IsEmpty(result.TypeDeclarations.Cast<object>().Concat(result.MemberDeclarations).Concat(result.NamespaceDeclarations));
			}
		}
	}
}
