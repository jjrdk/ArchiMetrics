// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberMetricsDataGridViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberMetricsDataGridViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;

	internal class MemberMetricsDataGridViewModel : ViewModelBase
	{
		private readonly IProjectMetricsRepository _metricsRepository;
		private readonly IAppContext _config;
		private int _memberCyclomaticComplexity;
		private int _depthOfInheritance;
		private double _memberMaintainabilityIndex;
		private IList<IMemberMetric> _memberMetrics;
		private int _linesOfCode;

		public MemberMetricsDataGridViewModel(
			IProjectMetricsRepository metricsRepository,
			IAppContext config)
			: base(config)
		{
			_memberMetrics = new List<IMemberMetric>();
			_metricsRepository = metricsRepository;
			_config = config;
			UpdateInternal();
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
			MemberMaintainabilityIndex = LinesOfCode == 0 ? 0 : (memberMetrics.Sum(x => x.LinesOfCode * x.MaintainabilityIndex) / LinesOfCode);
			MemberCyclomaticComplexity = LinesOfCode == 0 ? 0 : (memberMetrics.Sum(x => x.LinesOfCode * x.CyclomaticComplexity) / LinesOfCode);
			DepthOfInheritance = depthOfInheritance;
			MemberMetrics = memberMetrics;
			IsLoading = false;
		}
	}
}
