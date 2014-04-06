namespace ArchiMetrics.Common.Metrics
{
	using System.Threading.Tasks;
	using Microsoft.CodeAnalysis;

	public interface IProjectMetricsCalculator
	{
		Task<IProjectMetric> Calculate(Project project, Solution solution);
	}
}