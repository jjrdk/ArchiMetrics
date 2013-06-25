// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertionReport.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the AssertionReport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.ExcelWriter.Reports
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using CodeReview;
	using Common;
	using OfficeOpenXml;

	public class AssertionReport : IReportJob
	{
		private ICodeErrorRepository _errorRepository;
		private Task _workTask;

		public AssertionReport(ICodeErrorRepository errorRepository)
		{
			_errorRepository = errorRepository;
		}

		public Task AddReport(ExcelPackage package, ReportConfig config)
		{
			return Task.Factory.StartNew(() => GenerateReport(package, config));
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~AssertionReport()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_errorRepository.Dispose();
				_errorRepository = null;
				if (_workTask != null)
				{
					_workTask.Dispose();
					_workTask = null;
				}
			}
		}

		private void GenerateReport(ExcelPackage package, ReportConfig config)
		{
			var assertionGrouping = new Func<int, string>(x =>
															  {
																  if (x == 1)
																  {
																	  return "a. 1";
																  }

																  if (x == 0
																	  || x <= 4)
																  {
																	  return "b. x <= 4";
																  }

																  return "c. > 4";
															  });
			Console.WriteLine("Starting Test Assertion Report");
			var ws = package.Workbook.Worksheets.Add("Test Asserts - " + ReportUtils.GetMonth());
			var reviewTasks =
				config.Projects.SelectMany(
					setting =>
					setting.Roots
						   .SelectMany(root => _errorRepository.GetErrors(root.Source, root.IsTest)
															   .Where(x => x.Comment == "Multiple asserts found in test.")
															   .GroupBy(x => assertionGrouping(x.ErrorCount))
															   .Select(x => new Tuple<string, string, int>(setting.Name, x.Key, x.Count()))));

			WriteWorksheet(ws, reviewTasks);

			Console.WriteLine("Finishing Test Assertion Report");
		}

		private static void WriteWorksheet(ExcelWorksheet worksheet, IEnumerable<Tuple<string, string, int>> results)
		{
			Console.WriteLine("Writing assert report.");
			var dict = results
				.GroupBy(x => x.Item1, x => new KeyValuePair<string, int>(x.Item2, x.Item3))
				.ToDictionary(x => x.Key, x =>
					{
						var testCount = x.Sum(z => z.Value);
						return x.Select(y => new Tuple<string, int, int>(y.Key, y.Value, testCount)).ToList();
					});
			var allAsserts = dict.Values.SelectMany(t => t.Select(x => x.Item1)).Distinct().OrderBy(x => x).ToArray();
			worksheet.Cells[1, 1].Value = "Asserts per test";
			for (var i = 0; i < dict.Keys.Count; i++)
			{
				worksheet.Cells[1, i + 2].Value = dict.Keys.ElementAt(i);
			}

			for (var j = 0; j < allAsserts.Length; j++)
			{
				var row = j + 2;
				var value = allAsserts[j];
				worksheet.Cells[row, 1].Value = value;
				for (var i = 0; i < dict.Keys.Count; i++)
				{
					var list = dict[dict.Keys.ElementAt(i)];
					var testCount = list.Select(x => x.Item3).FirstOrDefault();
					var count = list.Where(x => x.Item1 == value).Select(x => x.Item2).FirstOrDefault();
					worksheet.Cells[row, i + 2].Formula = string.Format("= {0} / {1}", count, testCount);
				}
			}

			Console.WriteLine("Finished assert counting.");
		}
	}
}
