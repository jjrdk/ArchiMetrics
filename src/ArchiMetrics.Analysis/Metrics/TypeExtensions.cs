// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Common.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal static class TypeExtensions
	{
		public static string GetName(this TypeDeclarationSyntax syntax)
		{
			var containingTypeName = string.Join(".", GetContainingTypeName(syntax).Reverse());
			if (syntax.TypeParameterList != null)
			{
				var parameters = syntax.TypeParameterList.Parameters;
				if (parameters.Any())
				{
					var str3 = string.Join(", ", from x in parameters select x.Identifier.ValueText);
					containingTypeName = containingTypeName + $"<{str3}>";
				}
			}

			return containingTypeName;
		}

		public static ITypeDefinition GetQualifiedName(this ITypeSymbol symbol)
		{
			var name = string.Join(".", GetContainingTypeName(symbol).Reverse());

			var namedTypeSymbol = symbol as INamedTypeSymbol;
			if (namedTypeSymbol != null && namedTypeSymbol.TypeParameters != null && namedTypeSymbol.TypeParameters.Any())
			{
				var joined = string.Join(", ", namedTypeSymbol.TypeParameters.Select(x => x.Name));
				name = name + $"<{joined}>";
			}

			var namespaceNames = new List<string>();
			for (var containingSymbol = symbol.ContainingSymbol; (containingSymbol != null) && (containingSymbol.Kind == SymbolKind.Namespace); containingSymbol = containingSymbol.ContainingSymbol)
			{
				var namespaceSymbol = (INamespaceSymbol)containingSymbol;
				if (namespaceSymbol.IsGlobalNamespace)
				{
					return new TypeDefinition(name, string.Join(".", namespaceNames), namespaceSymbol.ContainingAssembly.Name);
				}

				namespaceNames.Add(namespaceSymbol.Name);
			}

			return new TypeDefinition(name, string.Join(".", namespaceNames), string.Empty);
		}

		private static IEnumerable<string> GetContainingTypeName(TypeDeclarationSyntax syntax)
		{
			for (var typeDeclaration = syntax; typeDeclaration != null; typeDeclaration = typeDeclaration.Parent as TypeDeclarationSyntax)
			{
				yield return typeDeclaration.Identifier.ValueText;
			}
		}

		private static IEnumerable<string> GetContainingTypeName(ITypeSymbol symbol)
		{
			for (var typeSymbol = symbol; typeSymbol != null; typeSymbol = typeSymbol.ContainingSymbol as ITypeSymbol)
			{
				yield return typeSymbol.Name;
			}
		}
	}
}
