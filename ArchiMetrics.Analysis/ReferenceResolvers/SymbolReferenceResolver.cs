// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolReferenceResolver.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SymbolReferenceResolver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.ReferenceResolvers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.CodeAnalysis;

	internal static class SymbolReferenceResolver
	{
		public static IEnumerable<IGrouping<ISymbol, Location>> Resolve(this Compilation compilation, SyntaxNode root)
		{
			var model = compilation.GetSemanticModel(root.SyntaxTree);
		
			var fields = root.DescendantNodes()
				.Select(
					x =>
					{
						var symbol = model.GetSymbolInfo(x);
						return new { symbol = symbol.Symbol, node = x };
					})
				.Where(x => x.symbol != null)
				.Select(x => new { type = x.symbol, location = x.node.GetLocation() })
				.GroupBy(x => x.type, x => x.location);

			var array = fields.ToArray();

			return array;
		}
	}
}