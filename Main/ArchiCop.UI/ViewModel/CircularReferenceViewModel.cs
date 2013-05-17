// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularReferenceViewModel.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CircularReferenceViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiCop.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMeter.Analysis;
	using ArchiMeter.Common;

	internal class CircularReferenceViewModel : EdgesViewModelBase
	{
		private readonly DependencyAnalyzer _analyzer = new DependencyAnalyzer();
		private IEnumerable<DependencyChain> _circularReferences;

		public CircularReferenceViewModel(
			IEdgeItemsRepository repository, 
			IEdgeTransformer filter, 
			IVertexRuleDefinition ruleDefinition)
			: base(repository, filter, ruleDefinition)
		{
			CircularReferences = new List<DependencyChain>();
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
				if (value != _circularReferences)
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

			_analyzer.GetCircularReferences(edgeItems)
				.ContinueWith(t =>
					{
						if (t.Exception != null)
						{
							var exceptions = t.Exception.InnerExceptions;
							var exceptionCount = exceptions.Count;
							return;
						}

						CircularReferences = t.Result.ToArray();
						IsLoading = false;
					});
		}
	}
}
