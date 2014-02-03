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

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Input;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Structure;
	using ArchiMetrics.UI.Support;

	internal class GraphViewModel : ViewModelBase
	{
		private readonly ISyntaxTransformer _filter;
		private readonly IProvider<string, ObservableCollection<TransformRule>> _rulesProvider;
		private readonly IAppContext _config;
		private readonly IVertexRepository _repository;
		private readonly DelegateCommand _updateCommand;
		private IModelNode[] _allMetricsEdges;
		private ModelGraph _graphToVisualize;
		private CancellationTokenSource _tokenSource;

		public GraphViewModel(
			IVertexRepository repository,
			ISyntaxTransformer filter,
			IProvider<string, ObservableCollection<TransformRule>> rulesProvider,
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

		public ModelGraph GraphToVisualize
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

		private async void UpdateImpl(bool forceUpdate)
		{
			if (_tokenSource != null)
			{
				_tokenSource.Cancel();
				_tokenSource.Dispose();
			}

			_tokenSource = new CancellationTokenSource();
			if (forceUpdate)
			{
				await LoadAllEdges(_tokenSource.Token);
			}
			else
			{
				await UpdateInternal(_tokenSource.Token);
			}
		}

		private async Task UpdateInternal(CancellationToken cancellationToken)
		{
			IsLoading = true;

			var rules = _rulesProvider.Get(_config.RulesSource);

			var edges =
				(await _filter.Transform(_allMetricsEdges, rules, cancellationToken))
					.WhereNot(x => string.IsNullOrWhiteSpace(x.QualifiedName))
					.SelectMany(x => x.Flatten())
					.Where(e => !string.IsNullOrWhiteSpace(e.QualifiedName))
					.Where(x => x.IsShared())
					.SelectMany(vertex => vertex.Children.Where(y => y.IsShared()).Select(x => new ModelEdge(vertex, x)))
					.WhereNot(e => e.Target.Equals(e.Source))
					.TakeWhile(x => !cancellationToken.IsCancellationRequested)
					.ToArray();
			var g = new ModelGraph(edges);

			if (!cancellationToken.IsCancellationRequested)
			{
				GraphToVisualize = g;
			}

			IsLoading = false;
		}

		private async Task LoadAllEdges(CancellationToken cancellationToken)
		{
			IsLoading = true;
			_allMetricsEdges = (await _repository.GetVertices(_config.Path, cancellationToken)).ToArray();
			await UpdateInternal(cancellationToken);
		}
	}
}
