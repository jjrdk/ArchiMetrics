// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GraphViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.Common.Structure;

	internal class GraphViewModel : ViewModelBase
	{
		private readonly IEdgeTransformer _filter;
		private readonly IEdgeItemsRepository _repository;
		private MetricsEdgeItem[] _allMetricsEdges;
		private ProjectGraph _graphToVisualize;

		public GraphViewModel(IEdgeItemsRepository repository, IEdgeTransformer filter, ISolutionEdgeItemsRepositoryConfig config)
			: base(config)
		{
			_repository = repository;
			_filter = filter;
			LoadAllEdges();
		}

		public ProjectGraph GraphToVisualize
		{
			get
			{
				return _graphToVisualize;
			}

			private set
			{
				if (_graphToVisualize != value)
				{
					_graphToVisualize = value;
					RaisePropertyChanged();
				}
			}
		}

		protected override void Update(bool forceUpdate)
		{
			if (forceUpdate)
			{
				LoadAllEdges();
			}
			else
			{
				UpdateInternal();
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			_graphToVisualize = null;
			_allMetricsEdges = null;
			base.Dispose(isDisposing);
		}

		private async void UpdateInternal()
		{
			IsLoading = true;

			var nonEmptySourceItems = (await _filter.TransformAsync(_allMetricsEdges))
				.ToArray();

			//var circularReferences = (await DependencyAnalyzer.GetCircularReferences(nonEmptySourceItems))
			//	.ToArray();

			var projectVertices = nonEmptySourceItems
				.SelectMany(item =>
				{
					var isCircular = false; // circularReferences.Any(c => c.Contains(item));
					return CreateVertices(item, isCircular);
				})
				.GroupBy(v => v.Name)
				.Select(grouping => grouping.First())
				.ToArray();

			var edges =
				nonEmptySourceItems
					.Where(e => !string.IsNullOrWhiteSpace(e.Dependency))
					.Select(
						dependencyItemViewModel =>
							new ProjectEdge(
								projectVertices.First(item => item.Name == dependencyItemViewModel.Dependant),
								projectVertices.First(item => item.Name == dependencyItemViewModel.Dependency)))
					.Where(e => e.Target.Name != e.Source.Name);
			var g = new ProjectGraph();

			foreach (var vertex in projectVertices)
			{
				g.AddVertex(vertex);
			}

			foreach (var edge in edges)
			{
				g.AddEdge(edge);
			}

			GraphToVisualize = g;
			IsLoading = false;
		}

		private IEnumerable<Vertex> CreateVertices(MetricsEdgeItem item, bool isCircular)
		{
			yield return new Vertex(item.Dependant, isCircular, item.DependantComplexity, item.DependantMaintainabilityIndex, item.DependantLinesOfCode);
			if (!string.IsNullOrWhiteSpace(item.Dependency))
			{
				yield return
					new Vertex(item.Dependency, isCircular, item.DependencyComplexity, item.DependencyMaintainabilityIndex, item.DependencyLinesOfCode, item.CodeIssues);
			}
		}

		private void LoadAllEdges()
		{
			IsLoading = true;
			_repository.GetEdgesAsync()
				.ContinueWith(t =>
				{
					_allMetricsEdges = t.Result.Where(e => e.Dependant != e.Dependency).ToArray();
					UpdateInternal();
				});
		}
	}
}
