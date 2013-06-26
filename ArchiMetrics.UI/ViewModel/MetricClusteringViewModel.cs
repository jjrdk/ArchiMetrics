namespace ArchiCop.UI.ViewModel
{
	using ArchiMetrics.CodeReview.Intelligence.Clustering;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;

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
