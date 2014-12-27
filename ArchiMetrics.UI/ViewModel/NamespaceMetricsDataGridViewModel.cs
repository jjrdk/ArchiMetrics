// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceMetricsDataGridViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceMetricsDataGridViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;

	internal class NamespaceMetricsDataGridViewModel : ViewModelBase
	{
		private readonly IAppContext _config;
		private readonly IProjectMetricsRepository _metricsRepository;
		private int _linesOfCode;
		private int _maxDepthOfInheritance;
		private int _namespaceCyclomaticComplexity;
		private double _namespaceMaintainabilityIndex;
		private IList<INamespaceMetric> _namespaceMetrics = new List<INamespaceMetric>();

		public NamespaceMetricsDataGridViewModel(
			IProjectMetricsRepository metricsRepository,
			IAppContext config)
			: base(config)
		{
			_metricsRepository = metricsRepository;
			_config = config;
			UpdateInternal();
		}

		public int NamespaceCyclomaticComplexity
		{
			get
			{
				return _namespaceCyclomaticComplexity;
			}

			private set
			{
				if (!_namespaceCyclomaticComplexity.Equals(value))
				{
					_namespaceCyclomaticComplexity = value;
					RaisePropertyChanged();
				}
			}
		}

		public int MaxDepthOfInheritance
		{
			get
			{
				return _maxDepthOfInheritance;
			}

			private set
			{
				if (!_maxDepthOfInheritance.Equals(value))
				{
					_maxDepthOfInheritance = value;
					RaisePropertyChanged();
				}
			}
		}

		public double NamespaceMaintainabilityIndex
		{
			get
			{
				return _namespaceMaintainabilityIndex;
			}

			private set
			{
				if (!_namespaceMaintainabilityIndex.Equals(value))
				{
					_namespaceMaintainabilityIndex = value;
					RaisePropertyChanged();
				}
			}
		}

		public IList<INamespaceMetric> NamespaceMetrics
		{
			get
			{
				return _namespaceMetrics;
			}

			private set
			{
				if (!_namespaceMetrics.Equals(value))
				{
					_namespaceMetrics = value;
					RaisePropertyChanged();
				}
			}
		}

		public int LinesOfCode
		{
			get
			{
				return _linesOfCode;
			}

			private set
			{
				if (!_linesOfCode.Equals(value))
				{
					_linesOfCode = value;
					RaisePropertyChanged();
				}
			}
		}

		protected override void Update(bool forceUpdate)
		{
			UpdateInternal();
			base.Update(forceUpdate);
		}

		private async void UpdateInternal()
		{
			IsLoading = true;
			var solutionPath = _config.Path;
			var awaitable = _metricsRepository.Get(solutionPath).ConfigureAwait(false);
			var metricsTasks = (await awaitable).AsArray();

			var metrics = metricsTasks
				.SelectMany(x => x.NamespaceMetrics)
				.AsArray();
			var typeMetrics = metrics.SelectMany(x => x.TypeMetrics).AsArray();
			LinesOfCode = typeMetrics.Sum(x => x.LinesOfCode);
			var depthOfInheritance = metrics.Any() ? metrics.Max(x => x.DepthOfInheritance) : 0;
			NamespaceMaintainabilityIndex = LinesOfCode == 0 ? 0 : (typeMetrics.Sum(x => x.LinesOfCode * x.MaintainabilityIndex) / LinesOfCode);
			NamespaceCyclomaticComplexity = LinesOfCode == 0 ? 0 : (typeMetrics.Sum(x => x.LinesOfCode * x.CyclomaticComplexity) / LinesOfCode);
			MaxDepthOfInheritance = depthOfInheritance;
			NamespaceMetrics = metrics.ToList();
			IsLoading = false;
		}
	}
}