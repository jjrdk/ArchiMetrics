// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsDataGridViewModel.cs" company="Reimers.dk">
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
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;

	internal class MetricsDataGridViewModel : ViewModelBase
	{
		private readonly IProjectMetricsRepository _metricsRepository;
		private readonly IAppContext _config;
		private int _typeCyclomaticComplexity;
		private int _memberCyclomaticComplexity;
		private int _depthOfInheritance;
		private double _typeMaintainabilityIndex;
		private double _memberMaintainabilityIndex;
		private IList<IMemberMetric> _memberMetrics;
		private IList<ITypeMetric> _typeMetrics;
		private int _linesOfCode;

		public MetricsDataGridViewModel(
			IProjectMetricsRepository metricsRepository,
			IAppContext config)
			: base(config)
		{
			_memberMetrics = new List<IMemberMetric>();
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

		public int MemberCyclomaticComplexity
		{
			get
			{
				return _memberCyclomaticComplexity;
			}

			private set
			{
				if (!_memberCyclomaticComplexity.Equals(value))
				{
					_memberCyclomaticComplexity = value;
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

		public double MemberMaintainabilityIndex
		{
			get
			{
				return _memberMaintainabilityIndex;
			}

			private set
			{
				if (!_memberMaintainabilityIndex.Equals(value))
				{
					_memberMaintainabilityIndex = value;
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

		public IList<IMemberMetric> MemberMetrics
		{
			get
			{
				return _memberMetrics;
			}

			private set
			{
				if (!ReferenceEquals(_memberMetrics, value))
				{
					_memberMetrics = value;
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
			var memberMetrics = typeMetrics.SelectMany(x => x.MemberMetrics).ToArray();
			LinesOfCode = typeMetrics.Sum(x => x.LinesOfCode);
			var depthOfInheritance = metrics.Any() ? metrics.Max(x => x.DepthOfInheritance) : 0;
			TypeMaintainabilityIndex = LinesOfCode == 0 ? 0 : (typeMetrics.Sum(x => x.LinesOfCode * x.MaintainabilityIndex) / LinesOfCode);
			MemberMaintainabilityIndex = LinesOfCode == 0 ? 0 : (memberMetrics.Sum(x => x.LinesOfCode * x.MaintainabilityIndex) / LinesOfCode);
			TypeCyclomaticComplexity = LinesOfCode == 0 ? 0 : (typeMetrics.Sum(x => x.LinesOfCode * x.CyclomaticComplexity) / LinesOfCode);
			MemberCyclomaticComplexity = LinesOfCode == 0 ? 0 : (memberMetrics.Sum(x => x.LinesOfCode * x.CyclomaticComplexity) / LinesOfCode);
			DepthOfInheritance = depthOfInheritance;
			TypeMetrics = typeMetrics;
			MemberMetrics = memberMetrics;
			IsLoading = false;
		}
	}
}
