// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularReferenceViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CircularReferenceViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using Analysis;
	using Common;

	internal class CircularReferenceViewModel : EdgesViewModelBase
	{
		private readonly DependencyAnalyzer _analyzer = new DependencyAnalyzer();
		private IEnumerable<DependencyChain> _circularReferences;

		public CircularReferenceViewModel(
			IEdgeItemsRepository repository,
			IEdgeTransformer filter,
			IVertexRuleDefinition ruleDefinition,
			ISolutionEdgeItemsRepositoryConfig config)
			: base(repository, filter, ruleDefinition, config)
		{
			_circularReferences = new List<DependencyChain>();
			LoadEdges();
		}

		public IEnumerable<DependencyChain> CircularReferences
		{
			get
			{
				return _circularReferences;
			}

			private set
			{
				if (value != null && new HashSet<DependencyChain>(value).SetEquals(_circularReferences))
				{
					_circularReferences = value;
					RaisePropertyChanged();
				}
			}
		}

		protected async override void UpdateInternal()
		{
			IsLoading = true;
			var edgeItems = await Filter.TransformAsync(AllEdges);

			await _analyzer
				.GetCircularReferences(edgeItems)
				.ContinueWith(t =>
					{
						if (t.Exception != null)
						{
							IsLoading = false;
							return;
						}

						CircularReferences = t.Result.ToArray();
						IsLoading = false;
					});
		}
	}
}
