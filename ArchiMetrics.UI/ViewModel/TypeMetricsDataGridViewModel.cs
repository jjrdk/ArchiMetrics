// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMetricsDataGridViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeMetricsDataGridViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;

	internal class TypeMetricsDataGridViewModel : ViewModelBase
	{
		private readonly IAppContext _config;
		private readonly IProjectMetricsRepository _metricsRepository;
		private int _depthOfInheritance;
		private int _linesOfCode;
		private int _typeCyclomaticComplexity;
		private double _typeMaintainabilityIndex;
		private IList<ITypeMetric> _typeMetrics;

		public TypeMetricsDataGridViewModel(
			IProjectMetricsRepository metricsRepository, 
			IAppContext config)
			: base(config)
		{
			_metricsRepository = metricsRepository;
			_config = config;
			UpdateInternal();
		}

		public int TypeCyclomaticComplexity
		{
			get
			{
				return _typeCyclomaticComplexity;
			}

			private set
			{
				if (!_typeCyclomaticComplexity.Equals(value))
				{
					_typeCyclomaticComplexity = value;
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

		public double TypeMaintainabilityIndex
		{
			get
			{
				return _typeMaintainabilityIndex;
			}

			private set
			{
				if (!_typeMaintainabilityIndex.Equals(value))
				{
					_typeMaintainabilityIndex = value;
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

		public IList<ITypeMetric> TypeMetrics
		{
			get
			{
				return _typeMetrics;
			}

			private set
			{
				if (!ReferenceEquals(_typeMetrics, value))
				{
					_typeMetrics = value;
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
			var metricsTasks = (await _metricsRepository.Get(solutionPath)).ToArray();

			var metrics = metricsTasks
				.SelectMany(x => x.NamespaceMetrics)
				.ToArray();
			var typeMetrics = metrics.SelectMany(x => x.TypeMetrics).ToArray();
			LinesOfCode = typeMetrics.Sum(x => x.LinesOfCode);
			var depthOfInheritance = metrics.Any() ? metrics.Max(x => x.DepthOfInheritance) : 0;
			TypeMaintainabilityIndex = LinesOfCode == 0 ? 0 : (typeMetrics.Sum(x => x.LinesOfCode * x.MaintainabilityIndex) / LinesOfCode);
			TypeCyclomaticComplexity = LinesOfCode == 0 ? 0 : (typeMetrics.Sum(x => x.LinesOfCode * x.CyclomaticComplexity) / LinesOfCode);
			DepthOfInheritance = depthOfInheritance;
			TypeMetrics = typeMetrics;
			IsLoading = false;
		}
	}
}