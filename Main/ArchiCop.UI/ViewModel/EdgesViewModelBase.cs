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

namespace ArchiCop.UI.ViewModel
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
			_repository = repository;
			_filter = filter;
			VertexRules = ruleDefinition.VertexRules;
		}

		public ICollection<VertexRule> VertexRules { get; private set; }

		protected IEdgeTransformer Filter
		{
			get
			{
				return _filter;
			}
		}

		protected EdgeItem[] AllEdges
		{
			get
			{
				return _allEdges;
			}
		}

		public override void Update(bool forceReload)
		{
			if (forceReload)
			{
				LoadEdges();
			}
			else
			{
				UpdateInternal();
			}
		}

		protected async void LoadEdges()
		{
			IsLoading = true;
			_allEdges = (await _repository.GetEdgesAsync()).ToArray();
			UpdateInternal();
		}

		protected abstract void UpdateInternal();

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_allEdges = null;
			}

			base.Dispose(isDisposing);
		}
	}
}