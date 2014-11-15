// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SymbolExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System.Collections.Concurrent;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis.ReferenceResolvers;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.FindSymbols;
	using ReferencedSymbol = ArchiMetrics.Analysis.ReferenceResolvers.ReferencedSymbol;

	public static class SymbolExtensions
	{
		private static readonly ConcurrentDictionary<SolutionId, ReferenceRepository> KnownReferences = new ConcurrentDictionary<SolutionId, ReferenceRepository>();

		public static Task<ReferencedSymbol> FindReferences(this Solution solution, ISymbol symbol)
		{
			var repository = KnownReferences.GetOrAdd(solution.Id, x => new ReferenceRepository(solution));

			return Task.Run(
				() =>
					{
						var locations = repository.Get(symbol).ToArray();
						return new ReferencedSymbol(symbol, locations);
					});
		}
	}
}