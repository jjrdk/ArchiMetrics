// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeExtensionsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Metrics
{
	using System.Collections.Immutable;
	using System.Linq;
	using ArchiMetrics.Analysis.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Moq;
	using NUnit.Framework;

	public sealed class TypeExtensionsTests
	{
		[Test]
		public void WhenGettingNameFromNestedTypeDeclarationSyntaxThenGetsFullName()
		{
			const string ContainerName = "ContainerType";
			const string InnerName = "InnerType";

			var innerType = SyntaxFactory.TypeDeclaration(SyntaxKind.ClassDeclaration, SyntaxFactory.Identifier(InnerName));

			var declaration = SyntaxFactory.TypeDeclaration(
				SyntaxKind.ClassDeclaration, 
				SyntaxFactory.List<AttributeListSyntax>(), 
				SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)), 
				SyntaxFactory.Token(SyntaxKind.ClassKeyword), 
				SyntaxFactory.Identifier(ContainerName), 
				SyntaxFactory.TypeParameterList(), 
				SyntaxFactory.BaseList(), 
				SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(), 
				SyntaxFactory.Token(SyntaxKind.OpenBraceToken), 
				SyntaxFactory.List<MemberDeclarationSyntax>(new[] { innerType }), 
				SyntaxFactory.Token(SyntaxKind.CloseBraceToken), 
				SyntaxFactory.Token(SyntaxKind.SemicolonToken));

			var actualName = declaration.DescendantNodes().OfType<TypeDeclarationSyntax>().First().GetName();

			Assert.AreEqual(string.Concat(ContainerName, ".", InnerName), actualName);
		}

		[Test]
		public void WhenGettingNameFromGenericTypeDeclarationSyntaxThenIncludesTypeParameters()
		{
			const string ContainerName = "ContainerType";

			var clause = SyntaxFactory.TypeParameter("object");
			var declaration = SyntaxFactory.TypeDeclaration(
				SyntaxKind.ClassDeclaration, 
				SyntaxFactory.List<AttributeListSyntax>(), 
				SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)), 
				SyntaxFactory.Token(SyntaxKind.ClassKeyword), 
				SyntaxFactory.Identifier(ContainerName), 
				SyntaxFactory.TypeParameterList(SyntaxFactory.SeparatedList(new[] { clause })), 
				SyntaxFactory.BaseList(), 
				SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(), 
				SyntaxFactory.Token(SyntaxKind.OpenBraceToken), 
				SyntaxFactory.List<MemberDeclarationSyntax>(), 
				SyntaxFactory.Token(SyntaxKind.CloseBraceToken), 
				SyntaxFactory.Token(SyntaxKind.SemicolonToken));

			var actualName = declaration.GetName();

			Assert.AreEqual(string.Concat(ContainerName, "<object>"), actualName);
		}

		[Test]
		public void WhenGettingQualifiedNameFromITypeSymbolThenReturnsNameWithNamespace()
		{
			var containingNamespace = new Mock<INamespaceSymbol>();
			containingNamespace.SetupGet(x => x.Name).Returns("MyNamespace");
			containingNamespace.SetupGet(x => x.IsGlobalNamespace).Returns(false);
			containingNamespace.SetupGet(x => x.Kind).Returns(SymbolKind.Namespace);
			containingNamespace.SetupGet(x => x.ContainingSymbol).Returns((INamespaceSymbol)null);

			var mockTypeParameter = new Mock<ITypeParameterSymbol>();
			mockTypeParameter.SetupGet(x => x.Name).Returns("object");
			var readOnlyArray = ImmutableArray.Create(mockTypeParameter.Object);
			var mockTypeSymbol = new Mock<INamedTypeSymbol>();
			mockTypeSymbol.SetupGet(x => x.Name).Returns("TypeName");
			mockTypeSymbol.SetupGet(x => x.ContainingSymbol).Returns(containingNamespace.Object);
			mockTypeSymbol.SetupGet(x => x.TypeParameters).Returns(readOnlyArray);

			const string ExpectedQualifiedName = "MyNamespace.TypeName<object>, ";
			var actualQualifiedName = mockTypeSymbol.Object.GetQualifiedName();

			Assert.AreEqual(ExpectedQualifiedName, actualQualifiedName.ToString());
		}

		[Test]
		public void WhenGettingQualifiedNameFromITypeSymbolWithAssemblyInfoThenReturnsNameWithNamespaceAndAssembly()
		{
			var containingAssembly = new Mock<IAssemblySymbol>();
			containingAssembly.SetupGet(x => x.Name).Returns("MyAssembly");

			var globalNamespace = new Mock<INamespaceSymbol>();
			globalNamespace.SetupGet(x => x.Kind).Returns(SymbolKind.Namespace);
			globalNamespace.SetupGet(x => x.IsGlobalNamespace).Returns(true);
			globalNamespace.SetupGet(x => x.ContainingAssembly).Returns(containingAssembly.Object);

			var containingNamespace = new Mock<INamespaceSymbol>();
			containingNamespace.SetupGet(x => x.Name).Returns("MyNamespace");
			containingNamespace.SetupGet(x => x.IsGlobalNamespace).Returns(false);
			containingNamespace.SetupGet(x => x.Kind).Returns(SymbolKind.Namespace);
			containingNamespace.SetupGet(x => x.ContainingSymbol).Returns(globalNamespace.Object);

			var mockTypeParameter = new Mock<ITypeParameterSymbol>();
			mockTypeParameter.SetupGet(x => x.Name).Returns("object");
			var readOnlyArray = ImmutableArray.Create(mockTypeParameter.Object);
			var mockTypeSymbol = new Mock<INamedTypeSymbol>();
			mockTypeSymbol.SetupGet(x => x.Name).Returns("TypeName");
			mockTypeSymbol.SetupGet(x => x.ContainingSymbol).Returns(containingNamespace.Object);
			mockTypeSymbol.SetupGet(x => x.TypeParameters).Returns(readOnlyArray);

			const string ExpectedQualifiedName = "MyNamespace.TypeName<object>, MyAssembly";
			var actualQualifiedName = mockTypeSymbol.Object.GetQualifiedName();

			Assert.AreEqual(ExpectedQualifiedName, actualQualifiedName.ToString());
		}
	}
}
