namespace ArchiMeter.ExcelWriter.Reports
{
	using System;
	using Common;
	using Common.Documents;
	using Raven.Repositories;

	public class TestMetricsReport : MetricsReportBase
	{
		public TestMetricsReport(IFactory<Func<ProjectInventoryDocument, string[]>, MetricsProvider> metricsProviderFactory)
			: base(metricsProviderFactory, d => d.TestProjectNames, 'T')
		{
		}
	}
}