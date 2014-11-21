﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReferenceRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArchiMetrics.Common;
using Microsoft.CodeAnalysis;

namespace ArchiMetrics.Analysis.ReferenceResolvers
{
	public class ReferenceRepository : IProvider<ISymbol, IEnumerable<Location>>
	{
		private readonly object _syncRoot = new object();
		private readonly ConcurrentDictionary<ISymbol, IEnumerable<Location>> _resolvedReferences = new ConcurrentDictionary<ISymbol, IEnumerable<Location>>();
		private bool _isInitialized;

		public ReferenceRepository(Solution solution)
		{
			var task = Scan(solution);
		}

		public IEnumerable<Location> Get(ISymbol key)
		{
			lock (_syncRoot)
			{
				while (!_isInitialized)
				{
					Monitor.Wait(_syncRoot);
				}
			}

			IEnumerable<Location> locations;
			return _resolvedReferences.TryGetValue(key, out locations)
				? locations
				: Enumerable.Empty<Location>();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_resolvedReferences.Clear();
			}
		}

		private async Task Scan(Solution solution)
		{
			var roots = await GetDocData(solution).ConfigureAwait(false);

			var groups = from root in roots
						 from syntaxNode in root.DocRoots
						 let compilation = root.Compilation
						 from @group in compilation.Resolve(syntaxNode)
						 select @group;

			foreach (var @group in groups)
			{
				_resolvedReferences.AddOrUpdate(@group.Key, @group.ToArray(), (s, r) => r.Concat(@group).ToArray());
			}

			lock (_syncRoot)
			{
				_isInitialized = true;
				Monitor.PulseAll(_syncRoot);
			}
		}

		private async Task<IEnumerable<DocData>> GetDocData(Solution solution)
		{
			var roots = (from project in solution.Projects
						 let compilation = project.GetCompilationAsync()
						 let docRoots = project.Documents.Select(x => x.GetSyntaxRootAsync())
						 select new { compilation, docRoots }).ToArray();

			await Task.WhenAll(roots.SelectMany(x => new Task[] { x.compilation }.Concat(x.docRoots)));

			return roots.Select(x => new DocData
			{
				Compilation = x.compilation.Result,
				DocRoots = x.docRoots.Select(y => y.Result).ToArray()
			});
		}

		private class DocData
		{
			public Compilation Compilation { get; set; }

			public IEnumerable<SyntaxNode> DocRoots { get; set; }
		}
	}
}