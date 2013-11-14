// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularReferenceViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using System.Threading;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.Common.Structure;

	internal class CircularReferenceViewModel : EdgesViewModelBase
	{
		private IEnumerable<DependencyChain> _circularReferences;

		public CircularReferenceViewModel(
			IEdgeItemsRepository repository,
			IEdgeTransformer filter,
			IVertexRuleDefinition ruleDefinition,
			ISolutionEdgeItemsRepositoryConfig config)
			: base(repository, filter, ruleDefinition, config)
		{
			_circularReferences = new List<DependencyChain>();
			UpdateImpl(true);
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

		protected async override void UpdateInternal(CancellationToken cancellationToken)
		{
			IsLoading = true;
			var edgeItems = await Filter.Transform(AllMetricsEdges, cancellationToken);

			await DependencyAnalyzer.GetCircularReferences(edgeItems, cancellationToken)
				.ContinueWith(
					t =>
					{
						if (t.IsCanceled)
						{
							return;
						}

						if (t.Exception != null)
						{
							IsLoading = false;
							return;
						}

						CircularReferences = t.Result.ToArray();
						IsLoading = false;
					},
					cancellationToken);
		}
	}
}
