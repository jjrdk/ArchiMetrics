// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgesViewModelBase.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EdgesViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;

	using ArchiMeter.Common;

	public abstract class EdgesViewModelBase : WorkspaceViewModel
	{
		private readonly IEdgeTransformer _filter;
		private readonly IEdgeItemsRepository _repository;
		private EdgeItem[] _allEdges;

		public EdgesViewModelBase(IEdgeItemsRepository repository, IEdgeTransformer filter, IVertexRuleDefinition ruleDefinition)
		{
			this._repository = repository;
			this._filter = filter;
			this.VertexRules = ruleDefinition.VertexRules;
		}

		public ICollection<VertexRule> VertexRules { get; private set; }

		protected IEdgeTransformer Filter
		{
			get
			{
				return this._filter;
			}
		}

		protected EdgeItem[] AllEdges
		{
			get
			{
				return this._allEdges;
			}
		}

		public override void Update(bool forceReload)
		{
			if (forceReload)
			{
				this.LoadEdges();
			}
			else
			{
				this.UpdateInternal();
			}
		}

		protected async void LoadEdges()
		{
			this.IsLoading = true;
			this._allEdges = (await this._repository.GetEdgesAsync()).ToArray();
			this.UpdateInternal();
		}

		protected abstract void UpdateInternal();

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this._allEdges = null;
			}

			base.Dispose(isDisposing);
		}
	}
}