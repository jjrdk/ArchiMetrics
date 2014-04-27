// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMetricsDataGridViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectMetricsDataGridViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;

	internal class ProjectMetricsDataGridViewModel : ViewModelBase
	{
		private readonly IAppContext _config;
		private readonly IProjectMetricsRepository _metricsRepository;
		private int _linesOfCode;
		private int _maxDepthOfInheritance;
		private int _projectCyclomaticComplexity;
		private double _projectMaintainabilityIndex;
		private IList<IProjectMetric> _projectMetrics = new List<IProjectMetric>();

		public ProjectMetricsDataGridViewModel(
			IProjectMetricsRepository metricsRepository, 
			IAppContext config)
			: base(config)
		{
			_metricsRepository = metricsRepository;
			_config = config;
			UpdateInternal();
		}

		public int ProjectCyclomaticComplexity
		{
			get
			{
				return _projectCyclomaticComplexity;
			}

			private set
			{
				if (!_projectCyclomaticComplexity.Equals(value))
				{
					_projectCyclomaticComplexity = value;
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

		public double ProjectMaintainabilityIndex
		{
			get
			{
				return _projectMaintainabilityIndex;
			}

			private set
			{
				if (!_projectMaintainabilityIndex.Equals(value))
				{
					_projectMaintainabilityIndex = value;
					RaisePropertyChanged();
				}
			}
		}

		public IList<IProjectMetric> ProjectMetrics
		{
			get
			{
				return _projectMetrics;
			}

			private set
			{
				if (!_projectMetrics.Equals(value))
				{
					_projectMetrics = value;
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
			var metricsTasks = (await awaitable).ToArray();

			var metrics = metricsTasks
				.SelectMany(x => x.NamespaceMetrics)
				.ToArray();
			var typeMetrics = metrics.SelectMany(x => x.TypeMetrics).ToArray();
			LinesOfCode = typeMetrics.Sum(x => x.LinesOfCode);
			var depthOfInheritance = metrics.Any() ? metrics.Max(x => x.DepthOfInheritance) : 0;
			ProjectMaintainabilityIndex = LinesOfCode == 0 ? 0 : (typeMetrics.Sum(x => x.LinesOfCode * x.MaintainabilityIndex) / LinesOfCode);
			ProjectCyclomaticComplexity = LinesOfCode == 0 ? 0 : (typeMetrics.Sum(x => x.LinesOfCode * x.CyclomaticComplexity) / LinesOfCode);
			MaxDepthOfInheritance = depthOfInheritance;
			ProjectMetrics = metricsTasks.ToList();
			IsLoading = false;
		}
	}
}