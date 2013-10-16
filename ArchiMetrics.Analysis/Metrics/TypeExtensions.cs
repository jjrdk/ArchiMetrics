// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
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
	using System.Text;
	using ArchiMetrics.Common.Metrics;
	using Roslyn.Compilers;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal static class TypeExtensions
	{
		public static string GetName(this TypeDeclarationSyntax syntax)
		{
			var valueText = syntax.Identifier.ValueText;
			var containingTypeName = GetContainingTypeName(syntax.Parent);
			if (!string.IsNullOrWhiteSpace(containingTypeName))
			{
				valueText = containingTypeName + "." + valueText;
			}

			var builder = new StringBuilder();
			builder.Append(valueText);
			if (syntax.TypeParameterList != null)
			{
				var parameters = syntax.TypeParameterList.Parameters;
				if (parameters.Any())
				{
					var str3 = string.Join(", ", from x in parameters select x.Identifier.ValueText);
					builder.AppendFormat("<{0}>", str3);
				}
			}

			return builder.ToString();
		}

		public static TypeDefinition GetQualifiedName(this ITypeSymbol symbol)
		{
			var name = symbol.Name;
			var containingTypeName = GetContainingTypeName(symbol.ContainingSymbol);
			if (!string.IsNullOrWhiteSpace(containingTypeName))
			{
				name = containingTypeName + "." + name;
			}

			var namedTypeSymbol = symbol as NamedTypeSymbol;
			if (namedTypeSymbol != null && namedTypeSymbol.TypeParameters != null && namedTypeSymbol.TypeParameters.Any())
			{
				var values = namedTypeSymbol.TypeParameters.Select(x => x.Name).ToArray();
				var joined = string.Join(", ", values);
				name = name + string.Format("<{0}>", joined);
			}

			var namespaceNames = new List<string>();
			for (var containingSymbol = symbol.ContainingSymbol; (containingSymbol != null) && (containingSymbol.Kind == CommonSymbolKind.Namespace); containingSymbol = containingSymbol.ContainingSymbol)
			{
				var namespaceSymbol = (NamespaceSymbol)containingSymbol;
				if (namespaceSymbol.IsGlobalNamespace)
				{
					return new TypeDefinition(name, string.Join(".", namespaceNames), namespaceSymbol.ContainingAssembly.Name);
				}

				namespaceNames.Add(namespaceSymbol.Name);
			}

			return new TypeDefinition(name, string.Join(".", namespaceNames), string.Empty);
		}

		private static string GetContainingTypeName(ISymbol symbol)
		{
			var symbol2 = symbol as NamedTypeSymbol;
			if (symbol2 == null)
			{
				return null;
			}

			var name = symbol2.Name;
			var containingTypeName = GetContainingTypeName(symbol2.ContainingSymbol);
			if (!string.IsNullOrWhiteSpace(containingTypeName))
			{
				return containingTypeName + "." + name;
			}

			return name;
		}

		private static string GetContainingTypeName(CommonSyntaxNode syntax)
		{
			var syntax2 = syntax as TypeDeclarationSyntax;
			if (syntax2 == null)
			{
				return null;
			}

			var valueText = syntax2.Identifier.ValueText;
			var containingTypeName = GetContainingTypeName(syntax2.Parent);
			if (!string.IsNullOrWhiteSpace(containingTypeName))
			{
				return containingTypeName + "." + valueText;
			}

			return valueText;
		}
	}
}
