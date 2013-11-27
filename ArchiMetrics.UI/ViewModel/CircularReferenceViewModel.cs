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

using ArchiMetrics.Common;

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.Common.Structure;

	internal class CircularReferenceViewModel : EdgesViewModelBase
	{
		private readonly IProvider<string, IEnumerable<VertexTransform>> _rulesProvider;
		private readonly IAppContext _config;
		private IEnumerable<DependencyChain> _circularReferences;

		public CircularReferenceViewModel(
			IEdgeItemsRepository repository,
			IEdgeTransformer filter,
			IProvider<string, IEnumerable<VertexTransform>> rulesProvider,
			IAppContext config)
			: base(repository, filter, config)
		{
			_rulesProvider = rulesProvider;
			_config = config;
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

		protected override async void UpdateInternal(CancellationToken cancellationToken)
		{
			try
			{
				IsLoading = true;
				var rules = _rulesProvider.Get(_config.RulesSource);
				var edgeItems = await Filter.Transform(AllMetricsEdges, rules, cancellationToken);

				var circularReferences = await DependencyAnalyzer.GetCircularReferences(edgeItems, cancellationToken);

				CircularReferences = circularReferences.ToArray();
			}
			finally
			{
				IsLoading = false;
			}
		}
	}
}
