// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductionCodeReviewReport.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProductionCodeReviewReport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.ExcelWriter.Reports
{
	using System;
	using Common;
	using Common.Documents;
	using Raven.Repositories;

	public class ProductionCodeReviewReport : CodeReviewReportBase
	{
		public ProductionCodeReviewReport(IFactory<Func<ProjectInventoryDocument, string[]>, ErrorDataProvider> errorDataRepository)
			: base(errorDataRepository, d => d.ProductionProjectNames, 'P')
		{
		}
	}
}
