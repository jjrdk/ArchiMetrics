// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsReport.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TestMetricsReport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Reports
{
	using System;

	using ArchiMeter.Common.Documents;

	using Common;

	using Raven.Repositories;

	public class TestMetricsReport : MetricsReportBase
	{
		public TestMetricsReport(IFactory<Func<ProjectInventoryDocument, string[]>, MetricsProvider> metricsProviderFactory)
			: base(metricsProviderFactory, d => d.TestProjectNames, 'T')
		{
		}
	}

	public class ProductionMetricsReport : MetricsReportBase
	{
		public ProductionMetricsReport(IFactory<Func<ProjectInventoryDocument, string[]>, MetricsProvider> metricsProviderFactory)
			: base(metricsProviderFactory, d => d.ProductionProjectNames, 'P')
		{
		}
	}
}