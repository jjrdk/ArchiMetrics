// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgesViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EdgesViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.ObjectModel;
	using System.Threading;
	using System.Windows.Data;
	using System.Windows.Input;
	using ArchiMetrics.Common.Structure;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.Common;

	public class EdgesViewModel : EdgesViewModelBase
	{
		private readonly IProvider<string, ObservableCollection<VertexTransform>> _rulesProvider;
		private readonly IAppContext _config;
		private readonly object _syncLock = new object();
		private readonly DelegateCommand _updateCommand;
		private ObservableCollection<MetricsEdgeItem> _dependencyItems;

		public EdgesViewModel(
			IEdgeItemsRepository repository,
			IProvider<string, ObservableCollection<VertexTransform>> rulesProvider,
			IEdgeTransformer filter,
			IAppContext config)
			: base(repository, filter, config)
		{
			_rulesProvider = rulesProvider;
			_config = config;
			DependencyItems = new ObservableCollection<MetricsEdgeItem>();
			UpdateImpl(true);
			_updateCommand = new DelegateCommand(o => true, o => UpdateImpl(false));
		}

		public ObservableCollection<MetricsEdgeItem> DependencyItems
		{
			get
			{
				return _dependencyItems;
			}

			private set
			{
				if (value != _dependencyItems)
				{
					if (_dependencyItems != null)
					{
						BindingOperations.DisableCollectionSynchronization(_dependencyItems);
					}

					_dependencyItems = value;
					if (_dependencyItems != null)
					{
						BindingOperations.EnableCollectionSynchronization(_dependencyItems, _syncLock);
					}

					RaisePropertyChanged();
				}
			}
		}

		public ICommand UpdateList
		{
			get
			{
				return _updateCommand;
			}
		}

		protected async override void UpdateInternal(CancellationToken cancellationToken)
		{
			IsLoading = true;
			var rules = _rulesProvider.Get(_config.RulesSource);
			var results = await Filter.Transform(AllMetricsEdges, rules, cancellationToken);
			var newCollection = new ObservableCollection<MetricsEdgeItem>(results);
			VertexTransforms = rules;
			DependencyItems = newCollection;
			IsLoading = false;
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_dependencyItems.Clear();
			}

			base.Dispose(isDisposing);
		}
	}
}
