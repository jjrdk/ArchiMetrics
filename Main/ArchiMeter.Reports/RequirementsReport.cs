// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequirementsReport.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RequirementsReport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Reports
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Analysis;
	using CodeReview;
	using Common;
	using OfficeOpenXml;

	public class RequirementsReport : IReportJob
	{
		private IRequirementTestAnalyzer _analyzer;

		public RequirementsReport(IRequirementTestAnalyzer analyzer)
		{
			_analyzer = analyzer;
		}

		public Task AddReport(ExcelPackage package, ReportConfig config)
		{
			return Task.Factory.StartNew(() =>
											 {
												 Console.WriteLine("Starting Requirement Testing Report");
												 var worksheet = package.Workbook.Worksheets.Add("Requirements Testing - " + ReportUtils.GetMonth());
												 var reportTasks = config.Projects
																		 .Select((project, 
																				  i) =>
																				 new Tuple<int, string, IEnumerable<RequirementToTestReport>>(
																					 i + 2, 
																					 project.Name, 
																					 project.Roots.SelectMany(root => _analyzer.GetRequirementTests(root.Source))));
												 WriteWorksheet(worksheet, reportTasks);
												 Console.WriteLine("Finished Requirement Testing Report");
											 });
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_analyzer = null;
			}
		}

		~RequirementsReport()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}

		private static void WriteWorksheet(ExcelWorksheet worksheet, IEnumerable<Tuple<int, string, IEnumerable<RequirementToTestReport>>> reports)
		{
			worksheet.Cells[1, 1].Value = "Requirement Testing";
			worksheet.Cells[2, 1].Value = "Tests per Requirement";
			worksheet.Cells[3, 1].Value = "Unique Tests per Requirement";
			worksheet.Cells[4, 1].Value = "Max Asserts per Test";
			worksheet.Cells[5, 1].Value = "Min Asserts per Test";
			worksheet.Cells[6, 1].Value = "Average Asserts per Test";

			foreach (var report in reports)
			{
				Console.WriteLine(report.Item2);
				var column = report.Item1;
				var requirementReports = report.Item3.ToArray();
				var totalAsserts = requirementReports.Select(r => new KeyValuePair<int, int>(r.RequirementId, r.AssertsPerTest.Sum(x => x.Item2))).ToArray();
				worksheet.Cells[1, column].Value = report.Item2;
				worksheet.Cells[2, column].Value = requirementReports.Length == 0 ? 0 : requirementReports.Select(x => x.CoveringTests.Count()).Sum() / requirementReports.Length;
				worksheet.Cells[3, column].Value = requirementReports.Length == 0 ? 0 : requirementReports.Select(x => x.CoveringTestNames.Except(requirementReports.Where(y => y.RequirementId != x.RequirementId).SelectMany(y => y.CoveringTestNames).Distinct()).Count()).Sum() / requirementReports.Length;
				worksheet.Cells[4, column].Value = totalAsserts.Length == 0 ? 0 : totalAsserts.Max(x => x.Value);
				worksheet.Cells[5, column].Value = totalAsserts.Length == 0 ? 0 : totalAsserts.Min(x => x.Value);
				worksheet.Cells[6, column].Value = totalAsserts.Length == 0 ? 0 : totalAsserts.Average(x => x.Value);
			}
		}
	}
}
