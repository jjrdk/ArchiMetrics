// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgesViewModel.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EdgesViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.UI.ViewModel
{
	using System.Collections.ObjectModel;
	using System.Windows.Data;

	using ArchiMeter.Common;

	public class EdgesViewModel : EdgesViewModelBase
	{
		private readonly object _syncLock = new object();
		private ObservableCollection<EdgeItem> _dependencyItems;

		public EdgesViewModel(IEdgeItemsRepository repository, IEdgeTransformer filter, IVertexRuleDefinition ruleDefinition)
			: base(repository, filter, ruleDefinition)
		{
			this.DependencyItems = new ObservableCollection<EdgeItem>();
			this.LoadEdges();
		}

		public ObservableCollection<EdgeItem> DependencyItems
		{
			get
			{
				return this._dependencyItems;
			}

			private set
			{
				if (value != this._dependencyItems)
				{
					if (this._dependencyItems != null)
					{
						BindingOperations.DisableCollectionSynchronization(this._dependencyItems);
					}

					this._dependencyItems = value;
					if (this._dependencyItems != null)
					{
						BindingOperations.EnableCollectionSynchronization(this._dependencyItems, this._syncLock);
					}

					this.RaisePropertyChanged();
				}
			}
		}

		protected override async void UpdateInternal()
		{
			this.IsLoading = true;
			this.DependencyItems.Clear();
			foreach (var item in await this.Filter.TransformAsync(this.AllEdges))
			{
				this.DependencyItems.Add(item);
			}

			this.IsLoading = false;
		}

		protected override void Dispose(bool isDisposing)
		{if (isDisposing)
		{
			this._dependencyItems.Clear();
		}

			base.Dispose(isDisposing);
		}
	}
}