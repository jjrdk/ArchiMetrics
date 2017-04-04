// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolReferenceResolver.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SymbolReferenceResolver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.ReferenceResolvers
{
	using System.Collections.Generic;
	using System.Linq;
	using Common;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal static class SymbolReferenceResolver
	{
		public static IEnumerable<IGrouping<ISymbol, ReferenceLocation>> Resolve(this Compilation compilation, SyntaxNode root)
		{
			var model = compilation.GetSemanticModel(root.SyntaxTree);

			var fields = root.DescendantNodes()
				.Select(
					descendant =>
					{
						var symbol = model.GetSymbolInfo(descendant);
						return new KeyValuePair<ISymbol, SyntaxNode>(symbol.Symbol, descendant);
					})
				.Where(x => x.Key != null)
				.SelectMany(x => GetSymbolDetails(x, model))
				.GroupBy(x => x.Symbol, x => x.Location);

			var array = fields.AsArray();

			return array;
		}

		private static IEnumerable<SymbolDetails> GetSymbolDetails(KeyValuePair<ISymbol, SyntaxNode> x, SemanticModel model)
		{
			var containingType = ResolveContainingType(x.Value, model);
			yield return new SymbolDetails(x.Key, new ReferenceLocation(x.Value.GetLocation(), containingType, model));

			var namedType = x.Key as INamedTypeSymbol;
			if (namedType != null && namedType.ConstructedFrom != null && namedType.ConstructedFrom != x.Key)
			{
				yield return new SymbolDetails(namedType.ConstructedFrom, new ReferenceLocation(x.Value.GetLocation(), containingType, model));
			}

			var namedMethod = x.Key as IMethodSymbol;
			if (namedMethod != null && namedMethod.ConstructedFrom != null && namedMethod.ConstructedFrom != x.Key)
			{
				yield return new SymbolDetails(namedMethod, new ReferenceLocation(x.Value.GetLocation(), ResolveContainingType(x.Value, model), model));
			}
		}

		private static ITypeSymbol ResolveContainingType(SyntaxNode node, SemanticModel model)
		{
			if (node == null)
			{
				return null;
			}

			var parent = node.Parent;
			if (parent is BaseTypeDeclarationSyntax)
			{
				var symbolInfo = model.GetDeclaredSymbol(parent);
				return symbolInfo as ITypeSymbol;
			}

			return ResolveContainingType(parent, model);
		}

		private class SymbolDetails
		{
			public SymbolDetails(ISymbol symbol, ReferenceLocation location)
			{
				Symbol = symbol;
				Location = location;
			}

			public ISymbol Symbol { get; }

			public ReferenceLocation Location { get; }
		}
	}
}