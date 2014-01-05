// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using System.Linq;
	using ArchiMetrics.Analysis.Metrics;
	using Moq;
	using NUnit.Framework;
	using Roslyn.Compilers;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	public sealed class TypeExtensionsTests
	{
		[Test]
		public void WhenGettingNameFromNestedTypeDeclarationSyntaxThenGetsFullName()
		{
			const string ContainerName = "ContainerType";
			const string InnerName = "InnerType";

			var innerType = Syntax.TypeDeclaration(SyntaxKind.ClassDeclaration, Syntax.Identifier(InnerName));

			var declaration = Syntax.TypeDeclaration(
				SyntaxKind.ClassDeclaration,
				Syntax.List<AttributeListSyntax>(),
				Syntax.TokenList(Syntax.Token(SyntaxKind.PublicKeyword)),
				Syntax.Token(SyntaxKind.ClassKeyword),
				Syntax.Identifier(ContainerName),
				Syntax.TypeParameterList(),
				Syntax.BaseList(),
				Syntax.List<TypeParameterConstraintClauseSyntax>(),
				Syntax.Token(SyntaxKind.OpenBraceToken),
				Syntax.List<MemberDeclarationSyntax>(innerType),
				Syntax.Token(SyntaxKind.CloseBraceToken),
				Syntax.Token(SyntaxKind.SemicolonToken));

			var actualName = declaration.DescendantNodes().OfType<TypeDeclarationSyntax>().First().GetName();

			Assert.AreEqual(string.Concat(ContainerName, ".", InnerName), actualName);
		}

		[Test]
		public void WhenGettingNameFromGenericTypeDeclarationSyntaxThenIncludesTypeParameters()
		{
			const string ContainerName = "ContainerType";

			var clause = Syntax.TypeParameter("object");
			var declaration = Syntax.TypeDeclaration(
				SyntaxKind.ClassDeclaration,
				Syntax.List<AttributeListSyntax>(),
				Syntax.TokenList(Syntax.Token(SyntaxKind.PublicKeyword)),
				Syntax.Token(SyntaxKind.ClassKeyword),
				Syntax.Identifier(ContainerName),
				Syntax.TypeParameterList(Syntax.SeparatedList(clause)),
				Syntax.BaseList(),
				Syntax.List<TypeParameterConstraintClauseSyntax>(),
				Syntax.Token(SyntaxKind.OpenBraceToken),
				Syntax.List<MemberDeclarationSyntax>(),
				Syntax.Token(SyntaxKind.CloseBraceToken),
				Syntax.Token(SyntaxKind.SemicolonToken));

			var actualName = declaration.GetName();

			Assert.AreEqual(string.Concat(ContainerName, "<object>"), actualName);
		}

		[Test]
		public void WhenGettingQualifiedNameFromITypeSymbolThenReturnsNameWithNamespace()
		{
			var containingNamespace = new Mock<INamespaceSymbol>();
			containingNamespace.SetupGet(x => x.Name).Returns("MyNamespace");
			containingNamespace.SetupGet(x => x.IsGlobalNamespace).Returns(false);
			containingNamespace.SetupGet(x => x.Kind).Returns(CommonSymbolKind.Namespace);
			containingNamespace.SetupGet(x => x.ContainingSymbol).Returns((INamespaceSymbol)null);

			var mockTypeParameter = new Mock<ITypeParameterSymbol>();
			mockTypeParameter.SetupGet(x => x.Name).Returns("object");
			var readOnlyArray = ReadOnlyArray<ITypeParameterSymbol>.CreateFrom(mockTypeParameter.Object);
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
			globalNamespace.SetupGet(x => x.Kind).Returns(CommonSymbolKind.Namespace);
			globalNamespace.SetupGet(x => x.IsGlobalNamespace).Returns(true);
			globalNamespace.SetupGet(x => x.ContainingAssembly).Returns(containingAssembly.Object);

			var containingNamespace = new Mock<INamespaceSymbol>();
			containingNamespace.SetupGet(x => x.Name).Returns("MyNamespace");
			containingNamespace.SetupGet(x => x.IsGlobalNamespace).Returns(false);
			containingNamespace.SetupGet(x => x.Kind).Returns(CommonSymbolKind.Namespace);
			containingNamespace.SetupGet(x => x.ContainingSymbol).Returns(globalNamespace.Object);

			var mockTypeParameter = new Mock<ITypeParameterSymbol>();
			mockTypeParameter.SetupGet(x => x.Name).Returns("object");
			var readOnlyArray = ReadOnlyArray<ITypeParameterSymbol>.CreateFrom(mockTypeParameter.Object);
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
