// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequirementGraphViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RequirementGraphViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Structure;

	internal class RequirementGraphViewModel : ViewModelBase
	{
		private readonly IRequirementTestAnalyzer _analyzer;
		private readonly IAppContext _config;
		private readonly IEdgeTransformer _filter;
		private MetricsEdgeItem[] _allMetricsEdges;
		private ProjectGraph _graphToVisualize;
		private CancellationTokenSource _tokenSource;

		public RequirementGraphViewModel(IRequirementTestAnalyzer analyzer, IAppContext config, IEdgeTransformer filter)
			: base(config)
		{
			_analyzer = analyzer;
			_config = config;
			_filter = filter;
			UpdateImpl(true);
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
			UpdateImpl(forceUpdate);
		}

		private void UpdateImpl(bool forceUpdate)
		{
			if (_tokenSource != null)
			{
				_tokenSource.Cancel();
				_tokenSource.Dispose();
			}

			_tokenSource = new CancellationTokenSource();
			if (forceUpdate)
			{
				LoadAllEdges(_tokenSource.Token);
			}
			else
			{
				UpdateInternal(_tokenSource.Token);
			}
		}

		private async void UpdateInternal(CancellationToken cancellationToken)
		{
			IsLoading = true;
			var g = new ProjectGraph();

			var nonEmptySourceItems = (await _filter.Transform(_allMetricsEdges, cancellationToken))
				.ToArray();

			var projectVertices = nonEmptySourceItems
				.SelectMany(item => CreateVertices(item)
										.GroupBy(v => v.Name)
										.Select(grouping => grouping.First()))
				.ToArray();

			var edges =
				nonEmptySourceItems
				.Where(e => !string.IsNullOrWhiteSpace(e.Dependency))
				.Select(
					dependencyItemViewModel =>
					new ProjectEdge(
						projectVertices.First(item => item.Name == dependencyItemViewModel.Dependant),
						projectVertices.First(item => item.Name == dependencyItemViewModel.Dependency)))
								   .Where(e => e.Target.Name != e.Source.Name)
								   .ToList();

			foreach (var vertex in projectVertices)
			{
				g.AddVertex(vertex);
			}

			foreach (var edge in edges)
			{
				g.AddEdge(edge);
			}

			if (!cancellationToken.IsCancellationRequested)
			{
				GraphToVisualize = g;
			}

			IsLoading = false;
		}

		private IEnumerable<Vertex> CreateVertices(MetricsEdgeItem item)
		{
			yield return new Vertex(item.Dependant, false, item.DependantComplexity, item.DependantMaintainabilityIndex, item.DependantLinesOfCode);
			if (!string.IsNullOrWhiteSpace(item.Dependency))
			{
				yield return
					new Vertex(item.Dependency, false, item.DependencyComplexity, item.DependencyMaintainabilityIndex, item.DependencyLinesOfCode, item.CodeIssues);
			}
		}

		private async void LoadAllEdges(CancellationToken cancellationToken)
		{
			IsLoading = true;
			var edges = await Task.Factory.StartNew(() => _analyzer.GetTestData(_config.Path), cancellationToken);
			_allMetricsEdges = await Task.Factory.StartNew(() => edges.SelectMany(ConvertToEdgeItem).Where(e => e.Dependant != e.Dependency).Distinct(new RequirementsEqualityComparer()).ToArray(), cancellationToken);
			UpdateInternal(cancellationToken);
		}

		private IEnumerable<MetricsEdgeItem> ConvertToEdgeItem(TestData data)
		{
			return data.RequirementIds.SelectMany(
				i => data.RequirementIds.Except(new[]
												{
													i
												})
						 .Select(
							 o =>
							 new MetricsEdgeItem
							 {
								 Dependant = i.ToString(CultureInfo.InvariantCulture),
								 Dependency = o.ToString(CultureInfo.InvariantCulture),
								 CodeIssues = new EvaluationResult[0]
							 }));
		}

		private class RequirementsEqualityComparer : IEqualityComparer<MetricsEdgeItem>
		{
			public bool Equals(MetricsEdgeItem x, MetricsEdgeItem y)
			{
				return x == null
						   ? y == null
						   : y != null && ((x.Dependant == y.Dependant && x.Dependency == y.Dependency) || (x.Dependant == y.Dependency && x.Dependency == y.Dependant));
			}

			public int GetHashCode(MetricsEdgeItem obj)
			{
				return string.Join(";", new[] { obj.Dependant, obj.Dependency }.OrderBy(x => x)).GetHashCode();
			}
		}
	}
}
