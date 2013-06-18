namespace ArchiMeter.ScriptPack
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMeter.CodeReview.Metrics;
	using ArchiMeter.Common.Metrics;
	using Roslyn.Services;
	using ScriptCs.Contracts;

	public class ArchiTools : IScriptPackContext
	{
		public Task<IEnumerable<NamespaceMetric>> CalculateMetrics(string path)
		{
			var calculator = new ProjectMetricsCalculator();
			var project = Workspace.LoadStandAloneProject(path)
				.CurrentSolution.Projects.Last();
			return calculator.Calculate(project);
		}
	}
}
