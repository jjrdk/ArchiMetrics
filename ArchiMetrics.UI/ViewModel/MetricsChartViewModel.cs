// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsChartViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MetricsChartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;

	internal class MetricsChartViewModel : ViewModelBase
	{
		private readonly IAppContext _config;
		private readonly IProjectMetricsRepository _repository;
		private IList<KeyValuePair<string, double>> _errorsByComplexity;
		private IList<KeyValuePair<string, double>> _errorsByLinesOfCode;
		private IList<KeyValuePair<string, double>> _errorsByMaintainability;
		private CancellationTokenSource _tokenSource;

		public MetricsChartViewModel(
			IProjectMetricsRepository repository,
			IAppContext config)
			: base(config)
		{
			_repository = repository;
			_config = config;
#pragma warning disable 4014
			UpdateInternal(true);
#pragma warning restore 4014
		}

		public IList<KeyValuePair<string, double>> MetricsByComplexity
		{
			get
			{
				return _errorsByComplexity;
			}

			private set
			{
				_errorsByComplexity = value;
				RaisePropertyChanged();
			}
		}

		public IList<KeyValuePair<string, double>> MetricsByMaintainability
		{
			get
			{
				return _errorsByMaintainability;
			}

			private set
			{
				_errorsByMaintainability = value;
				RaisePropertyChanged();
			}
		}

		public IList<KeyValuePair<string, double>> MetricsByLinesOfCode
		{
			get
			{
				return _errorsByLinesOfCode;
			}

			private set
			{
				_errorsByLinesOfCode = value;
				RaisePropertyChanged();
			}
		}

		protected async override void Update(bool forceUpdate)
		{
			await UpdateInternal(forceUpdate).ConfigureAwait(false);
		}

		protected override void Dispose(bool isDisposing)
		{
			base.Dispose(isDisposing);
			if (isDisposing)
			{
				_repository.Dispose();
			}
		}

		private static Tuple<int, string> CreateComplexityString(int complexity)
		{
			if (complexity <= 1)
			{
				return new Tuple<int, string>(1, "Complexity  = 1");
			}

			if (complexity <= 5)
			{
				return new Tuple<int, string>(2, "Complexity <=  5");
			}

			if (complexity <= 10)
			{
				return new Tuple<int, string>(3, "Complexity <= 10");
			}

			if (complexity <= 30)
			{
				return new Tuple<int, string>(4, "Complexity <= 30");
			}

			return new Tuple<int, string>(5, "Complexity > 30");
		}

		private static Tuple<int, string> CreateLocString(int loc)
		{
			if (loc <= 1)
			{
				return new Tuple<int, string>(1, "Lines of Code <=  1");
			}

			if (loc <= 10)
			{
				return new Tuple<int, string>(2, "Lines of Code <=  10");
			}

			if (loc <= 20)
			{
				return new Tuple<int, string>(3, "Lines of Code <=  20");
			}

			if (loc <= 30)
			{
				return new Tuple<int, string>(4, "Lines of Code <=  30");
			}

			if (loc <= 50)
			{
				return new Tuple<int, string>(5, "Lines of Code <=  50");
			}

			if (loc <= 100)
			{
				return new Tuple<int, string>(6, "Lines of Code <= 100");
			}

			return new Tuple<int, string>(7, "Lines of Code > 100");
		}

		private static string CreateMaintainabilityTitle(int number, double factor)
		{
			var maintainability = number * factor;
			return maintainability.Equals(100.0)
			? "Maintainability = 100"
			: string.Format("Maintainability  >= {0}", maintainability);
		}

		private async Task UpdateInternal(bool forceUpdate)
		{
			IsLoading = true;
			if (_tokenSource != null)
			{
				_tokenSource.Cancel(false);
				_tokenSource.Dispose();
			}

			_tokenSource = new CancellationTokenSource();
			base.Update(forceUpdate);
			if (forceUpdate)
			{
				var awaitable = _repository.Get(_config.Path).ConfigureAwait(false);
				var result = (await awaitable).AsArray();
				if (!_tokenSource.IsCancellationRequested)
				{
					var metrics = result.SelectMany(x => x.NamespaceMetrics);
					await DisplayMetrics(metrics).ConfigureAwait(false);
				}
			}
		}

		private async Task DisplayMetrics(IEnumerable<INamespaceMetric> metrics)
		{
			IsLoading = true;
			var results = metrics
				.SelectMany(x => x.TypeMetrics)
				.SelectMany(x => x.MemberMetrics)
				.AsArray();
			var linesOfCode = results.Sum(x => x.LinesOfCode);
			var complexityTask = DisplayErrorsByComplexity(results, linesOfCode);
			var linesOfCodeTask = DisplayErrorsByLinesOfCode(results, linesOfCode);
			var maintainabilityTask = DisplayErrorsByMaintainability(results, linesOfCode);

			await Task.WhenAll(complexityTask, linesOfCodeTask, maintainabilityTask).ConfigureAwait(false);
			IsLoading = false;
		}

		private Task DisplayErrorsByComplexity(IEnumerable<IMemberMetric> metrics, int linesOfCode)
		{
			return Task.Factory.StartNew(() =>
				{
					var results = metrics
						.GroupBy(x => CreateComplexityString(x.CyclomaticComplexity))
						.OrderBy(x => x.Key.Item1)
						.Select(x => new KeyValuePair<string, double>(x.Key.Item2, Math.Round((100.0 * x.Sum(y => y.LinesOfCode)) / linesOfCode, 2)))
						.OrderBy(x => x.Key)
						.ToList();
					MetricsByComplexity = results;
				});
		}

		private Task DisplayErrorsByLinesOfCode(IEnumerable<IMemberMetric> metrics, int linesOfCode)
		{
			return Task.Factory.StartNew(() =>
				{
					var results = metrics
						.GroupBy(x => CreateLocString(x.LinesOfCode))
						.OrderBy(x => x.Key.Item1)
						.Select(x => new KeyValuePair<string, double>(x.Key.Item2, Math.Round((100.0 * x.Sum(y => y.LinesOfCode)) / linesOfCode, 2)))
						.OrderBy(x => x.Key)
						.ToList();
					MetricsByLinesOfCode = results;
				});
		}

		private Task DisplayErrorsByMaintainability(IEnumerable<IMemberMetric> result, int linesOfCode)
		{
			return Task.Factory.StartNew(() =>
				{
					const double Factor = 10.0;
					var results = result
						.GroupBy(x => (int)Math.Floor(x.MaintainabilityIndex / Factor))
						.Select(x => new KeyValuePair<string, double>(CreateMaintainabilityTitle(x.Key, Factor), Math.Round((100.0 * x.Sum(y => y.LinesOfCode)) / linesOfCode, 2)))
						.OrderBy(x => x.Key)
						.ToList();
					MetricsByMaintainability = results;
				});
		}
	}
}