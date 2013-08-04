// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgeTransformer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
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
	using System.Threading.Tasks;
	using ArchiMetrics.Common;

	public class EdgeTransformer : IEdgeTransformer, IDisposable
	{
		private readonly ICollectionCopier _copier;
		private readonly ConcurrentDictionary<string, Regex> _regexes = new ConcurrentDictionary<string, Regex>();
		private readonly IVertexRuleRepository _ruleRepository;

		public EdgeTransformer(IVertexRuleRepository ruleRepository, ICollectionCopier copier)
		{
			_ruleRepository = ruleRepository;
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

		public async Task<IEnumerable<EdgeItem>> TransformAsync(IEnumerable<EdgeItem> source)
		{
			var copy = await _copier.Copy(source);

			var items = copy
				.AsParallel()
				.Select(item =>
					{
						foreach (var transform in _ruleRepository.GetAllVertexPreTransforms())
						{
							item.Dependant = transform(item.Dependant);
							item.Dependency = transform(item.Dependency);
						}

						foreach (var rule in _ruleRepository.VertexRules
														   .ToArray()
														   .Where(x => !string.IsNullOrWhiteSpace(x.Pattern)))
						{
							var regex = _regexes.GetOrAdd(
								rule.Pattern, 
								pattern => new Regex(pattern, RegexOptions.Compiled));
							item.Dependant = regex.Replace(item.Dependant, rule.Name ?? string.Empty);
							item.Dependency = regex.Replace(item.Dependency, rule.Name ?? string.Empty);
						}

						foreach (var transform in _ruleRepository.GetAllVertexPostTransforms())
						{
							item.Dependant = transform(item.Dependant);
							item.Dependency = transform(item.Dependency);
						}

						return item;
					})
				.AsSequential()
				.GroupBy(e => e.ToString())
				.Select(g =>
					{
						var first = g.First();
						return new EdgeItem
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

			return items;
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
