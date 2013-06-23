// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeNameResolver.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeNameResolver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Roslyn.Compilers.CSharp;

	internal sealed class TypeNameResolver
	{
		public static string GetName(TypeDeclarationSyntax syntax)
		{
			string valueText = syntax.Identifier.ValueText;
			string containingTypeName = GetContainingTypeName(syntax.Parent);
			if (!string.IsNullOrWhiteSpace(containingTypeName))
			{
				valueText = containingTypeName + "." + valueText;
			}

			var builder = new StringBuilder();
			builder.Append(valueText);
			if (syntax.TypeParameterList != null)
			{
				SeparatedSyntaxList<TypeParameterSyntax> parameters = syntax.TypeParameterList.Parameters;
				if (parameters.Any())
				{
					string str3 = string.Join(", ", from x in parameters select x.Identifier.ValueText);
					builder.AppendFormat("<{0}>", str3);
				}
			}

			return builder.ToString();
		}

		public static string GetQualifiedName(TypeSymbol symbol)
		{
			string name = symbol.Name;
			string containingTypeName = GetContainingTypeName(symbol.ContainingSymbol);
			if (!string.IsNullOrWhiteSpace(containingTypeName))
			{
				name = containingTypeName + "." + name;
			}

			var symbol2 = (NamedTypeSymbol)symbol;
			if ((symbol2.TypeParameters != null) && symbol2.TypeParameters.Any())
			{
				IEnumerable<string> values = (from x in symbol2.TypeParameters.AsEnumerable() select x.Name).ToArray<string>();
				string str3 = string.Join(", ", values);
				name = name + string.Format("<{0}>", str3);
			}

			for (Symbol symbol3 = symbol.ContainingSymbol; (symbol3 != null) && (symbol3.Kind == SymbolKind.Namespace); symbol3 = symbol3.ContainingSymbol)
			{
				var symbol4 = (NamespaceSymbol)symbol3;
				if (symbol4.IsGlobalNamespace)
				{
					return name + string.Format(", {0}", symbol4.ContainingAssembly.Name);
				}

				name = symbol3.Name + "." + name;
			}

			return name;
		}

		// Methods
		private static string GetContainingTypeName(Symbol symbol)
		{
			var symbol2 = symbol as NamedTypeSymbol;
			if (symbol2 == null)
			{
				return null;
			}

			string name = symbol2.Name;
			string containingTypeName = GetContainingTypeName(symbol2.ContainingSymbol);
			if (!string.IsNullOrWhiteSpace(containingTypeName))
			{
				return containingTypeName + "." + name;
			}

			return name;
		}

		private static string GetContainingTypeName(SyntaxNode syntax)
		{
			var syntax2 = syntax as TypeDeclarationSyntax;
			if (syntax2 == null)
			{
				return null;
			}

			string valueText = syntax2.Identifier.ValueText;
			string containingTypeName = GetContainingTypeName(syntax2.Parent);
			if (!string.IsNullOrWhiteSpace(containingTypeName))
			{
				return containingTypeName + "." + valueText;
			}

			return valueText;
		}
	}
}