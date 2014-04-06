namespace ArchiMetrics.Common.Structure
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.Metrics;

	public interface IProjectMetricsRepository : IDisposable
	{
		Task<IEnumerable<IProjectMetric>> Get(string solutionPath);
	}
}