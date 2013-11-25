namespace ArchiMetrics.Common.Structure
{
	using System.Threading.Tasks;
	using ArchiMetrics.Common.Metrics;

	public interface IProjectMetricsRepository
	{
		Task<ProjectCodeMetrics> Get(string projectPath, string solutionPath);
	}
}