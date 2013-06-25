namespace ArchiCop.UI.ViewModel
{
	using ArchiMeter.CodeReview.Intelligence.Clustering;
	using ArchiMeter.Common;
	using ArchiMeter.Common.Metrics;

	public class MetricClusteringViewModel : WorkspaceViewModel
	{
		private readonly IAsyncReadOnlyRepository<CodeMetrics> _codeMetricsRepository;
		private readonly IClusteringCalculator<CodeMetrics> _codeClusteringCalculator;

		public MetricClusteringViewModel(
			IAsyncReadOnlyRepository<CodeMetrics> codeMetricsRepository,
			IClusteringCalculator<CodeMetrics> codeClusteringCalculator)
		{
			_codeMetricsRepository = codeMetricsRepository;
			_codeClusteringCalculator = codeClusteringCalculator;
		}
	}
}