// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReportJob.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IReportJob type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.ExcelWriter.Reports
{
	using System;
	using System.Threading.Tasks;

	using ArchiMeter.Common;

	using OfficeOpenXml;

	public interface IReportJob : IDisposable
	{
		Task AddReport(ExcelPackage package, ReportConfig config);
	}
}