// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcelReportWriter.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ExcelReportWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.ReportWriter
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Threading.Tasks;
	using Common;
	using OfficeOpenXml;
	using Reports;

	internal class ExcelReportWriter
	{
		private readonly IEnumerable<IReportJob> _reportJobs;

		public ExcelReportWriter(IEnumerable<IReportJob> reportJobs)
		{
			_reportJobs = reportJobs;
		}

		public async Task GenerateReport(ReportConfig config)
		{
			var outputFile = string.IsNullOrWhiteSpace(config.OutputFile) ? "Report.xlsx" : config.OutputFile;
			if (File.Exists(outputFile))
			{
				File.Delete(outputFile);
			}
			var excelPackage = new ExcelPackage(File.Open(outputFile, FileMode.Create));

			foreach (var job in _reportJobs)
			{
				await job.AddReport(excelPackage, config);
				job.Dispose();
			}

			excelPackage.Save();
			excelPackage.Dispose();
			Console.WriteLine("Reports saved.");
		}
	}
}