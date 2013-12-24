// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MetricsViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;
	using Roslyn.Services;

	public class MetricsViewModel : ViewModelBase
	{
		private readonly IAppContext _config;
		private readonly IProjectMetricsRepository _metricsRepository;
		private readonly IProvider<string, ISolution> _solutionProvider;
		private int _cyclomaticComplexity;
		private int _depthOfInheritance;
		private double _maintainabilityIndex;
		private IList<ITypeMetric> _metrics;

		public MetricsViewModel(
			IProjectMetricsRepository metricsRepository, 
			IProvider<string, ISolution> solutionProvider, 
			IAppContext config)
			: base(config)
		{
			_metrics = new List<ITypeMetric>();
			_metricsRepository = metricsRepository;
			_solutionProvider = solutionProvider;
			_config = config;
			UpdateInternal();
		}

		public int CyclomaticComplexity
		{
			get
			{
				return _cyclomaticComplexity;
			}

			private set
			{
				if (!_cyclomaticComplexity.Equals(value))
				{
					_cyclomaticComplexity = value;
					RaisePropertyChanged();
				}
			}
		}

		public int DepthOfInheritance
		{
			get
			{
				return _depthOfInheritance;
			}

			private set
			{
				if (!_depthOfInheritance.Equals(value))
				{
					_depthOfInheritance = value;
					RaisePropertyChanged();
				}
			}
		}

		public double MaintainabilityIndex
		{
			get
			{
				return _maintainabilityIndex;
			}

			private set
			{
				if (!_maintainabilityIndex.Equals(value))
				{
					_maintainabilityIndex = value;
					RaisePropertyChanged();
				}
			}
		}

		public IList<ITypeMetric> Metrics
		{
			get
			{
				return _metrics;
			}

			private set
			{
				if (!ReferenceEquals(_metrics, value))
				{
					_metrics = value;
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
			var solution = _solutionProvider.Get(solutionPath);
			var metricsTasks = solution.Projects.Select(p => _metricsRepository.Get(p.FilePath, solutionPath)).ToArray();
			await Task.WhenAll(metricsTasks);

			var metrics = metricsTasks
				.Where(x => x.Result != null)
				.Select(x => x.Result)
				.SelectMany(x => x.Metrics)
				.ToArray();
			var loc = metrics.Sum(x => x.LinesOfCode);
			var maintainabilityIndex = metrics.Sum(x => x.LinesOfCode * x.MaintainabilityIndex) / loc;
			var cyclomaticComplexity = metrics.Sum(x => x.LinesOfCode * x.CyclomaticComplexity) / loc;
			var depthOfInheritance = metrics.Max(x => x.DepthOfInheritance);
			MaintainabilityIndex = maintainabilityIndex;
			CyclomaticComplexity = cyclomaticComplexity;
			DepthOfInheritance = depthOfInheritance;
			Metrics = metrics.SelectMany(x => x.TypeMetrics).ToList();
			IsLoading = false;
		}
	}
}
