// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductionCodeReviewReport.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProductionCodeReviewReport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Reports
{
	using System;

	using ArchiMeter.Common.Documents;

	using Common;

	using Raven.Repositories;

	public class ProductionCodeReviewReport : CodeReviewReportBase
	{
		public ProductionCodeReviewReport(IFactory<Func<ProjectInventoryDocument, string[]>, ErrorDataProvider> errorDataRepository)
			: base(errorDataRepository, d => d.ProductionProjectNames, 'P')
		{
		}
	}
}