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
	using System;
	using System.Collections.Concurrent;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis.ReferenceResolvers;
	using Common;
	using Microsoft.CodeAnalysis;

	public static class SymbolExtensions
	{
		private static readonly ConcurrentDictionary<SolutionId, Lazy<ReferenceRepository>> KnownReferences = new ConcurrentDictionary<SolutionId, Lazy<ReferenceRepository>>();

		public static Task<ReferencedSymbol> FindReferences(this Solution solution, ISymbol symbol)
		{
			if (solution == null)
			{
				return Task.FromResult(new ReferencedSymbol(symbol, new ReferenceLocation[0]));
			}

			var lazyRepo = KnownReferences.GetOrAdd(solution.Id, x => new Lazy<ReferenceRepository>(() => new ReferenceRepository(solution), LazyThreadSafetyMode.ExecutionAndPublication));

			return Task.Run(
				() =>
				{
					var repo = lazyRepo.Value;
					var locations = repo.Get(symbol).AsArray();
					return new ReferencedSymbol(symbol, locations);
				});
		}
	}
}