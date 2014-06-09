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
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.FindSymbols;

	public static class SymbolExtensions
	{
		private static readonly object SyncRoot = new object();
		private static readonly Dictionary<SolutionId, IDictionary<ISymbol, Task<ReferencedSymbol[]>>> KnownReferences = new Dictionary<SolutionId, IDictionary<ISymbol, Task<ReferencedSymbol[]>>>();

		public static Task<IEnumerable<ReferencedSymbol>> FindReferences(this Solution solution, ISymbol symbol)
		{
			if (symbol == null)
			{
				return Task.FromResult(Enumerable.Empty<ReferencedSymbol>());
			}

			lock (SyncRoot)
			{
				IDictionary<ISymbol, Task<ReferencedSymbol[]>> dictionary;
				if (!KnownReferences.TryGetValue(solution.Id, out dictionary))
				{
					dictionary = new Dictionary<ISymbol, Task<ReferencedSymbol[]>>();
					KnownReferences.Add(solution.Id, dictionary);
				}

				Task<ReferencedSymbol[]> referenceTask;
				if (!dictionary.TryGetValue(symbol, out referenceTask))
				{
					referenceTask =
						SymbolFinder.FindReferencesAsync(symbol, solution)
							.ContinueWith(t => t.Exception != null ? new ReferencedSymbol[0] : t.Result.ToArray());
					dictionary.Add(symbol, referenceTask);
				}

				return referenceTask.ContinueWith(x => x.Result.AsEnumerable());
			}
		}
	}
}