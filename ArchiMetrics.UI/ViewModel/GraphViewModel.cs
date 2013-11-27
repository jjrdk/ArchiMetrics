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

using System.Collections.ObjectModel;
using ArchiMetrics.Common;

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Windows.Input;
	using ArchiMetrics.Common.Structure;
	using ArchiMetrics.UI.Support;

	internal class GraphViewModel : ViewModelBase
	{
		private readonly IEdgeTransformer _filter;
		private readonly IProvider<string, ObservableCollection<VertexTransform>> _rulesProvider;
		private readonly IAppContext _config;
		private readonly IEdgeItemsRepository _repository;
		private readonly DelegateCommand _updateCommand;
		private MetricsEdgeItem[] _allMetricsEdges;
		private ProjectGraph _graphToVisualize;
		private CancellationTokenSource _tokenSource;

		public GraphViewModel(
			IEdgeItemsRepository repository,
			IEdgeTransformer filter,
			IProvider<string, ObservableCollection<VertexTransform>> rulesProvider,
			IAppContext config)
			: base(config)
		{
			_repository = repository;
			_filter = filter;
			_rulesProvider = rulesProvider;
			_config = config;
			UpdateImpl(true);
			_updateCommand = new DelegateCommand(o => true, o => Update(true));
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

		public ICommand UpdateGraph
		{
			get
			{
				return _updateCommand;
			}
		}

		protected override void Update(bool forceUpdate)
		{
			UpdateImpl(forceUpdate);
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_graphToVisualize = null;
				_allMetricsEdges = null;
				if (_tokenSource != null)
				{
					_tokenSource.Dispose();
				}
			}

			base.Dispose(isDisposing);
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

			var rules = _rulesProvider.Get(_config.RulesSource);
			var nonEmptySourceItems = (await _filter.Transform(_allMetricsEdges, rules, cancellationToken)).ToArray();

			////var circularReferences = (await DependencyAnalyzer.GetCircularReferences(nonEmptySourceItems, cancellationToken))
			////	.ToArray();

			var projectVertices = nonEmptySourceItems
				.TakeWhile(x => !cancellationToken.IsCancellationRequested)
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
					.TakeWhile(x => !cancellationToken.IsCancellationRequested)
					.Select(
						dependencyItemViewModel =>
						new ProjectEdge(
							projectVertices.First(item => item.Name == dependencyItemViewModel.Dependant),
							projectVertices.First(item => item.Name == dependencyItemViewModel.Dependency)))
					.Where(e => e.Target.Name != e.Source.Name);
			var g = new ProjectGraph();

			foreach (var vertex in projectVertices
				.TakeWhile(x => !cancellationToken.IsCancellationRequested))
			{
				g.AddVertex(vertex);
			}

			foreach (var edge in edges
				.TakeWhile(x => !cancellationToken.IsCancellationRequested))
			{
				g.AddEdge(edge);
			}

			if (!cancellationToken.IsCancellationRequested)
			{
				GraphToVisualize = g;
			}

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

		private void LoadAllEdges(CancellationToken cancellationToken)
		{
			IsLoading = true;
			_repository.GetEdges(_config.Path, _config.IncludeCodeReview, cancellationToken)
				.ContinueWith(
					t =>
					{
						_allMetricsEdges = t.Result.Where(e => e.Dependant != e.Dependency).ToArray();
						Update(false);
					},
					cancellationToken);
		}
	}
}
