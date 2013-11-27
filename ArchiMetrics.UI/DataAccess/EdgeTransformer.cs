// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgeTransformer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EdgeTransformer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.DataAccess
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Structure;

	public class EdgeTransformer : IEdgeTransformer, IDisposable
	{
		private readonly ICollectionCopier _copier;
		private readonly ConcurrentDictionary<string, Regex> _regexes = new ConcurrentDictionary<string, Regex>();

		public EdgeTransformer(ICollectionCopier copier)
		{
			_copier = copier;
		}

		~EdgeTransformer()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public async Task<IEnumerable<MetricsEdgeItem>> Transform(IEnumerable<MetricsEdgeItem> source, IEnumerable<VertexTransform> rules, CancellationToken cancellationToken)
		{
			var copy = await _copier.Copy(source, cancellationToken);

			var items = copy
				.Select(item =>
					{
						foreach (var rule in rules.Where(x => !string.IsNullOrWhiteSpace(x.Pattern)))
						{
							var regex = _regexes.GetOrAdd(rule.Pattern, pattern => new Regex(pattern, RegexOptions.Compiled));
							item.Dependant = regex.Replace(item.Dependant, rule.Name ?? string.Empty);
							item.Dependency = regex.Replace(item.Dependency, rule.Name ?? string.Empty);
						}
						
						return item;
					})
				.GroupBy(e => e.ToString())
				.Select(g =>
					{
						var first = g.First();
						return new MetricsEdgeItem
								   {
									   Dependant = first.Dependant,
									   Dependency = first.Dependency,
									   CodeIssues = first.CodeIssues,
									   MergedEdges = g.Count(),
									   DependantLinesOfCode = first.DependantLinesOfCode,
									   DependantComplexity = first.DependantComplexity,
									   DependantMaintainabilityIndex = first.DependantMaintainabilityIndex,
									   DependencyLinesOfCode = first.DependencyLinesOfCode,
									   DependencyComplexity = first.DependencyComplexity,
									   DependencyMaintainabilityIndex = first.DependencyMaintainabilityIndex
								   };
					})
				.Where(x => !string.IsNullOrWhiteSpace(x.Dependant))
				.Where(e => e.Dependant != e.Dependency)
				.ToArray()
				.AsEnumerable();

			return cancellationToken.IsCancellationRequested ? Enumerable.Empty<MetricsEdgeItem>() : items;
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_regexes.Clear();
			}
		}
	}
}
