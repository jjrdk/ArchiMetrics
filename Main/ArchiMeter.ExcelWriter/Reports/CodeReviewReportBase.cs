// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeReviewReportBase.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeReviewReportBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.ReportWriter.Reports
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Threading.Tasks;
	using CodeReview;
	using Common;
	using Common.Documents;
	using OfficeOpenXml;
	using Raven.Repositories;

	public abstract class CodeReviewReportBase : IReportJob
	{
		private readonly ErrorDataProvider _errorDataRepository;
		private readonly char _identifer;

		public CodeReviewReportBase(
			IFactory<Func<ProjectInventoryDocument, string[]>, ErrorDataProvider> errorDataRepository, 
			Func<ProjectInventoryDocument, string[]> filter, 
			char identifer)
		{
			_errorDataRepository = errorDataRepository.Create(filter);
			_identifer = identifer;
		}

		public Task AddReport(ExcelPackage package, ReportConfig config)
		{
			return Task.Factory.StartNew(() =>
										  {
											  Console.WriteLine("Starting Code Review");
											  
											  var results =
												  config.Projects.SelectMany(
													  setting => _errorDataRepository.GetErrors(setting.Name, setting.Revision.ToString(CultureInfo.InvariantCulture)))
														.ToArray();

											  var ws = package.Workbook.Worksheets.Add(string.Format("Code Review {0} - {1}", _identifer, ReportUtils.GetMonth()));
											  var relws = package.Workbook.Worksheets.Add(string.Format("Relative Review {0} - {1}", _identifer, ReportUtils.GetMonth()));

											  WriteWorksheet(ws, relws, results);
											  Console.WriteLine("Finished Code Review");
										  });
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~CodeReviewReportBase()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_errorDataRepository.Dispose();
			}
		}

		private void WriteWorksheet(ExcelWorksheet worksheet, ExcelWorksheet relativeWorksheet, IEnumerable<ErrorData> results)
		{
			var dict = results.GroupBy(x => x.ProjectName)
							  .ToDictionary(x => x.Key, 
											x => x.Where(y => y.Error != "Multiple asserts found in test." || !y.Occurrences.Equals(1))
												  .ToArray());
			var allErrors = dict.Values
								.SelectMany(t => t.Select(x => x.Error))
								.Distinct()
								.OrderBy(x => x)
								.ToArray();

			WriteValues(worksheet, dict, allErrors);
			WriteRelativeValues(relativeWorksheet, dict, allErrors);
		}

		private void WriteRelativeValues(
			ExcelWorksheet worksheet, 
			Dictionary<string, ErrorData[]> dict, 
			string[] allErrors)
		{
			if (dict.Count == 0)
			{
				worksheet.Cells[1, 1].Value = "No Errors";
				return;
			}

			worksheet.Cells[1, 1].Value = "Code Review";
			for (var i = 0; i < dict.Keys.Count; i++)
			{
				worksheet.Cells[1, i + 2].Value = dict.Keys.ElementAt(i);
			}

			for (int j = 0; j < allErrors.Length; j++)
			{
				var row = j + 2;
				var value = allErrors[j];
				worksheet.Cells[row, 1].Value = value;
				worksheet.Cells[allErrors.Length + 5, 1].Value = "GeoMean Factor";
				worksheet.Cells[allErrors.Length + 5, 2].Value = 0.000;
				for (var i = 0; i < dict.Keys.Count; i++)
				{
					var cell = worksheet.Cells[row, i + 2];
					cell.Style.Numberformat.Format = "0.00%";
					var address = cell.Address;

					var key = dict.Keys.ElementAt(i);
					var violations = dict[key].Where(x => x.Error == value)
											  .Select(x => x.Occurrences)
											  .FirstOrDefault();
					var affectedLoc = dict[key].Where(x => x.Error == value)
											   .Select(x => x.DistinctLoc)
											   .FirstOrDefault();
					cell.Formula = violations == 0
									   ? "= " + worksheet.Cells[allErrors.Length + 5, 2].Address
									   : string.Format(
										   "= ({3} / 'Code Metrics {0} - {1}'!{2}12) + {4}", 
										   _identifer, 
										   ReportUtils.GetMonth(), 
										   string.Join(string.Empty, address.TakeWhile(char.IsLetter)), 
										   affectedLoc, 
										   worksheet.Cells[allErrors.Length + 5, 2].Address);
				}
			}
		}

		private void WriteValues(
			ExcelWorksheet worksheet, 
			Dictionary<string, ErrorData[]> dict, 
			string[] allErrors)
		{
			if (dict.Count == 0)
			{
				worksheet.Cells[1, 1].Value = "No Errors";
				return;
			}

			worksheet.Cells[1, 1].Value = "Code Review";
			for (var i = 0; i < dict.Keys.Count; i++)
			{
				worksheet.Cells[1, i + 2].Value = dict.Keys.ElementAt(i);
			}

			for (int j = 0; j < allErrors.Length; j++)
			{
				var row = j + 2;
				var value = allErrors[j];
				worksheet.Cells[row, 1].Value = value;
				for (var i = 0; i < dict.Keys.Count; i++)
				{
					var cell = worksheet.Cells[row, i + 2];
					cell.Style.Numberformat.Format = "0.00%";
					var address = cell.Address;

					var key = dict.Keys.ElementAt(i);
					var violations = dict[key].Where(x => x.Error == value)
											  .Select(x => x.Occurrences)
											  .FirstOrDefault();
					var effort = dict[key].Where(x => x.Error == value)
										  .Select(x => x.Effort)
										  .FirstOrDefault();
					if (violations == 0)
					{
						cell.Value = 0;
					}
					else
					{
						cell.Formula = string.Format(
										 "= {3} / 'Code Metrics {0} - {1}'!{2}10", 
										 _identifer, 
										 ReportUtils.GetMonth(), 
										 string.Join(string.Empty, address.TakeWhile(char.IsLetter)), 
										 effort.ToString(CultureInfo.InvariantCulture));
					}
				}
			}

			var totalRow = allErrors.Length + 2;
			worksheet.Cells[totalRow, 1].Value = "Total";
			for (int i = 0; i < dict.Keys.Count; i++)
			{
				var cell = worksheet.Cells[totalRow, i + 2];
				cell.Formula = string.Format(
					"= SUM({0}:{1})", 
					worksheet.Cells[2, i + 2].Address, 
					worksheet.Cells[allErrors.Length + 1, i + 2].Address);
				cell.Style.Numberformat.Format = "0.00%";
			}
		}
	}
}