namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Microsoft.CodeAnalysis;

	public interface ICodeMetricsCalculator
	{
		Task<IEnumerable<INamespaceMetric>> Calculate(Project project, Solution solution);
	}
}
