namespace ArchiMeter.ExcelWriter.Reports
{
	using System;

	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;
	using ArchiMeter.Raven.Repositories;

	public class TestMetricsReport : MetricsReportBase
	{
		public TestMetricsReport(IFactory<Func<ProjectInventoryDocument, string[]>, MetricsProvider> metricsProviderFactory)
			: base(metricsProviderFactory, d => d.TestProjectNames, 'T')
		{
		}
	}
}