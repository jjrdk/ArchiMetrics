// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCodeReviewReport.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TestCodeReviewReport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.ReportWriter.Reports
{
	using System;

	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;
	using ArchiMeter.Raven.Repositories;

	public class TestCodeReviewReport : CodeReviewReportBase
	{
		public TestCodeReviewReport(IFactory<Func<ProjectInventoryDocument, string[]>, ErrorDataProvider> errorDataRepository)
			: base(errorDataRepository, d => d.TestProjectNames, 'T')
		{
		}
	}
}