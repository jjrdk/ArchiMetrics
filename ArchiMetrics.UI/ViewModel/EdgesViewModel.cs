// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgesViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
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
	using System.Windows.Data;
	using System.Windows.Input;
	using Common;
	using Support;

	public class EdgesViewModel : EdgesViewModelBase
	{
		private readonly object _syncLock = new object();
		private readonly DelegateCommand _updateCommand;
		private ObservableCollection<EdgeItem> _dependencyItems;

		public EdgesViewModel(IEdgeItemsRepository repository, IEdgeTransformer filter, IVertexRuleDefinition ruleDefinition, ISolutionEdgeItemsRepositoryConfig config)
			: base(repository, filter, ruleDefinition, config)
		{
			DependencyItems = new ObservableCollection<EdgeItem>();
			LoadEdges();
			_updateCommand = new DelegateCommand(o => true, o => UpdateInternal());
		}

		public ObservableCollection<EdgeItem> DependencyItems
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

		protected async override void UpdateInternal()
		{
			IsLoading = true;

			var results = await Filter.TransformAsync(AllEdges);
			var newCollection = new ObservableCollection<EdgeItem>(results);
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
