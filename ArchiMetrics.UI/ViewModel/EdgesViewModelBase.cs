// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EdgesViewModelBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EdgesViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common;

	public abstract class EdgesViewModelBase : ViewModelBase
	{
		private readonly IEdgeTransformer _filter;
		private readonly IEdgeItemsRepository _repository;
		private EdgeItem[] _allEdges;

		public EdgesViewModelBase(IEdgeItemsRepository repository, IEdgeTransformer filter, IVertexRuleDefinition ruleDefinition, ISolutionEdgeItemsRepositoryConfig config)
			: base(config)
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

		protected override void Update(bool forceUpdate)
		{
			base.Update(forceUpdate);
			if (forceUpdate)
			{
				LoadEdges();
			}
			else
			{
				UpdateInternal();
			}
		}

		protected void LoadEdges()
		{
			IsLoading = true;
			_repository.GetEdgesAsync()
			           .ContinueWith(t =>
				           {
					           _allEdges = t.Result.ToArray();
					           UpdateInternal();
				           });
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
