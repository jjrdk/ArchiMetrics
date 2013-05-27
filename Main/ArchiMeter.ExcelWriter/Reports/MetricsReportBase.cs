// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsReportBase.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MetricsReportBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.ReportWriter.Reports
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Threading.Tasks;

	using ArchiMeter.CodeReview;
	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;
	using ArchiMeter.Common.Metrics;
	using ArchiMeter.Raven.Repositories;

	using OfficeOpenXml;

	public abstract class MetricsReportBase : IReportJob
	{
		private readonly char _identifier;
		private readonly MetricsProvider _metricsProvider;

		public MetricsReportBase(IFactory<Func<ProjectInventoryDocument, string[]>, MetricsProvider> metricsProviderFactory, Func<ProjectInventoryDocument, string[]> filter, char identifier)
		{
			this._metricsProvider = metricsProviderFactory.Create(filter);
			this._identifier = identifier;
		}

		public Task AddReport(ExcelPackage package, ReportConfig config)
		{
			return Task.Factory.StartNew(() =>
				{
					Console.WriteLine("Starting Metrics Calculation");
					var metricResults =
						config.Projects
							  .Select(p =>
								  {
									  var metrics = this._metricsProvider
										  .GetMetrics(p.Name, p.Revision.ToString(CultureInfo.InvariantCulture));

									  return new Tuple<string, int, IEnumerable<TypeMetric>>(
										  p.Name,
										  metrics.SourceLinesOfCode,
										  metrics.Metrics);
								  })
							  .ToArray();


					var metricSheet = package.Workbook.Worksheets.Add(string.Format("Code Metrics {0} - {1}", this._identifier, ReportUtils.GetMonth()));
					var complexitySheet = package.Workbook.Worksheets.Add(string.Format("Code Complexity {0} - {1}", this._identifier, ReportUtils.GetMonth()));
					var nonWeightedComplexitySheet = package.Workbook.Worksheets.Add(string.Format("Raw Type Complexity {0} - {1}", this._identifier, ReportUtils.GetMonth()));
					var methodComplexitySheet = package.Workbook.Worksheets.Add(string.Format("Method Complexity {0} - {1}", this._identifier, ReportUtils.GetMonth()));
					var nonWeightedMethodComplexitySheet = package.Workbook.Worksheets.Add(string.Format("Raw Method Complexity {0} - {1}", this._identifier, ReportUtils.GetMonth()));
					var maintainabilitySheet = package.Workbook.Worksheets.Add(string.Format("Code Maintainability {0} - {1}", this._identifier, ReportUtils.GetMonth()));
					var nonWeightedMaintainabilitySheet = package.Workbook.Worksheets.Add(string.Format("Raw Type Maintainability {0} - {1}", this._identifier, ReportUtils.GetMonth()));

					WriteMetricWorksheet(metricSheet, metricResults);
					WriteComplexitySheet(complexitySheet, metricResults);
					WriteNonWeightedComplexitySheet(nonWeightedComplexitySheet, metricResults);
					WriteMethodComplexitySheet(methodComplexitySheet, metricResults);
					WriteNonWeightedMethodComplexitySheet(nonWeightedMethodComplexitySheet, metricResults);
					WriteMaintainabilitySheet(maintainabilitySheet, metricResults);
					WriteNonWeigtedMaintainabilitySheet(nonWeightedMaintainabilitySheet, metricResults);

					Console.WriteLine("Finishing Metrics Calculation");
				});
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
			}
		}

		~MetricsReportBase()
		{
			// Simply call Dispose(false).
			this.Dispose(false);
		}

		private static void WriteComplexitySheet(ExcelWorksheet worksheet, Tuple<string, int, IEnumerable<TypeMetric>>[] metricResults)
		{
			var complexityGrouping = new Func<int, string>(d =>
			{
				if (d < 8)
				{
					return "a. x < 8";
				}

				if (d < 20)
				{
					return "b. 8 <= x < 20";
				}

				if (d < 50)
				{
					return "c. 20 <= x < 50";
				}

				if (d < 100)
				{
					return "d. 50 <= x < 100";
				}

				return "e. x >= 100";
			});
			var complexities = metricResults.SelectMany(r => r.Item3.Select(x => x.CyclomaticComplexity)).GroupBy(complexityGrouping).Select(x => x.Key).Distinct().OrderBy(x => x).ToArray();
			var projectComplexities = metricResults.ToDictionary(r => r.Item1, r => r.Item3.GroupBy(x => complexityGrouping(x.CyclomaticComplexity)).ToDictionary(g => g.Key, g => g.Sum(x => x.LinesOfCode)));
			var projectSizes = metricResults.ToDictionary(r => r.Item1, r => r.Item3.Sum(x => x.LinesOfCode));

			FillComplexities(worksheet, projectComplexities, complexities, projectSizes);
		}

		private static void WriteMethodComplexitySheet(ExcelWorksheet worksheet, Tuple<string, int, IEnumerable<TypeMetric>>[] metricResults)
		{
			var complexityGrouping = new Func<int, string>(d =>
			{
				if (d < 5)
				{
					return "a. x < 5";
				}

				if (d < 8)
				{
					return "b. 5 <= x < 8";
				}

				if (d < 10)
				{
					return "c. 8 <= x < 10";
				}

				if (d < 20)
				{
					return "d. 10 <= x < 20";
				}

				return "e. x >= 20";
			});

			var complexities = metricResults
				.SelectMany(m => m.Item3.SelectMany(t => t.MemberMetrics))
				.Select(m => m.CyclomaticComplexity)
				.GroupBy(complexityGrouping)
				.Select(x => x.Key)
				.Distinct()
				.OrderBy(x => x)
				.ToArray();
			var projectComplexities = metricResults.ToDictionary(
				r => r.Item1,
				r => r.Item3
					.Where(t => t.Kind != TypeMetricKind.Interface)
					  .SelectMany(t => t.MemberMetrics)
					  .GroupBy(x => complexityGrouping(x.CyclomaticComplexity))
					  .ToDictionary(g => g.Key, g => g.Sum(x => x.LinesOfCode)));
			var projectSizes = metricResults.ToDictionary(r => r.Item1, r => r.Item3.Sum(x => x.LinesOfCode));
			FillComplexities(worksheet, projectComplexities, complexities, projectSizes);
		}

		private static void WriteNonWeightedComplexitySheet(ExcelWorksheet worksheet, Tuple<string, int, IEnumerable<TypeMetric>>[] metricResults)
		{
			var complexityGrouping = new Func<int, string>(d =>
			{
				if (d < 8)
				{
					return "a. x < 8";
				}

				if (d < 20)
				{
					return "b. 8 <= x < 20";
				}

				if (d < 50)
				{
					return "c. 20 <= x < 50";
				}

				if (d < 100)
				{
					return "d. 50 <= x < 100";
				}

				return "e. x >= 100";
			});
			var complexities = metricResults
				.SelectMany(r => r.Item3.Select(x => x.CyclomaticComplexity))
				.GroupBy(complexityGrouping)
				.Select(x => x.Key)
				.Distinct()
				.OrderBy(x => x)
				.ToArray();
			var projectComplexities = metricResults
				.ToDictionary(r => r.Item1, r => r.Item3
					.GroupBy(x => complexityGrouping(x.CyclomaticComplexity))
					.ToDictionary(g => g.Key, g => g.Count()));
			var projectSizes = metricResults.ToDictionary(r => r.Item1, r => r.Item3.Count());

			FillComplexities(worksheet, projectComplexities, complexities, projectSizes);
		}

		private static void WriteNonWeightedMethodComplexitySheet(ExcelWorksheet worksheet, Tuple<string, int, IEnumerable<TypeMetric>>[] metricResults)
		{
			var complexityGrouping = new Func<int, string>(d =>
			{
				if (d < 5)
				{
					return "a. x < 5";
				}

				if (d < 8)
				{
					return "b. 5 <= x < 8";
				}

				if (d < 10)
				{
					return "c. 8 <= x < 10";
				}

				if (d < 20)
				{
					return "d. 10 <= x < 20";
				}

				return "e. x >= 20";
			});

			var complexities = metricResults
				.SelectMany(m => m.Item3.SelectMany(t => t.MemberMetrics))
				.Select(m => m.CyclomaticComplexity)
				.GroupBy(complexityGrouping)
				.Select(x => x.Key)
				.Distinct()
				.OrderBy(x => x)
				.ToArray();
			var projectComplexities = metricResults.ToDictionary(
				r => r.Item1,
				r => r.Item3
					.Where(t => t.Kind != TypeMetricKind.Interface)
					  .SelectMany(t => t.MemberMetrics)
					  .GroupBy(x => complexityGrouping(x.CyclomaticComplexity))
					  .ToDictionary(g => g.Key, g => g.Count()));
			var projectSizes = metricResults.ToDictionary(r => r.Item1, r => r.Item3.SelectMany(t => t.MemberMetrics).Count());
			FillComplexities(worksheet, projectComplexities, complexities, projectSizes);
		}

		private static void FillComplexities(ExcelWorksheet worksheet,
											 Dictionary<string, Dictionary<string, int>> projectComplexities,
											 string[] complexities,
											 Dictionary<string, int> projectSizes)
		{
			worksheet.Cells[1, 1].Value = "Lines of Code / Project";
			worksheet.Cells[2, 1].Value = "Complexity / Project";
			for (int j = 0; j < projectComplexities.Keys.Count; j++)
			{
				worksheet.Cells[2, 2 + j].Value = projectComplexities.Keys.ElementAt(j);
			}

			for (int j = 0; j < projectComplexities.Keys.Count; j++)
			{
				var projectKey = projectComplexities.Keys.ElementAt(j);
				var dictionary = projectComplexities[projectKey];
				worksheet.Cells[1, 2 + j].Value = dictionary.Sum(x => x.Value);
			}

			for (var i = 0; i < complexities.Length; i++)
			{
				worksheet.Cells[3 + i, 1].Value = complexities[i];

				for (int j = 0; j < projectComplexities.Keys.Count; j++)
				{
					var projectKey = projectComplexities.Keys.ElementAt(j);
					var dictionary = projectComplexities[projectKey];
					var key = complexities[i];
					var cell = worksheet.Cells[3 + i, 2 + j];
					cell.Style.Numberformat.Format = "0.00%";
					cell.Value = dictionary.ContainsKey(key) ? ((double)dictionary[key] / projectSizes[projectKey]) : 0;
				}
			}

			var row = complexities.Length + 5;
			worksheet.Cells[row, 1].Value = "Below 20";
			for (int j = 0; j < projectComplexities.Keys.Count; j++)
			{
				var col = j + 2;
				var range = worksheet.Cells[row, col];
				range.Formula = string.Format("= SUM({0}: {1})", worksheet.Cells[3, col].Address, worksheet.Cells[4, col].Address);
				range.Style.Numberformat.Format = "0.00%";
			}

			row = complexities.Length + 6;
			worksheet.Cells[row, 1].Value = "Below 40";
			for (int j = 0; j < projectComplexities.Keys.Count; j++)
			{
				var col = j + 2;
				var range = worksheet.Cells[row, col];
				range.Formula = string.Format("= SUM({0}: {1})", worksheet.Cells[3, col].Address, worksheet.Cells[5, col].Address);
				range.Style.Numberformat.Format = "0.00%";
			}
		}

		private static void WriteMaintainabilitySheet(ExcelWorksheet worksheet, Tuple<string, int, IEnumerable<TypeMetric>>[] metricResults)
		{
			var maintainabilityGrouping = new Func<double, string>(d =>
				{
					if (d > 80)
					{
						return "a. x > 80";
					}

					if (d > 60)
					{
						return "b. 80 >= x > 60";
					}

					if (d > 40)
					{
						return "c. 60 >= x > 40";
					}

					if (d > 20)
					{
						return "d. 40 >= x > 20";
					}

					return "e. <= 20";
				});
			var maintainabilities = metricResults.SelectMany(r => r.Item3.Select(x => x.MaintainabilityIndex)).GroupBy(maintainabilityGrouping).Select(g => g.Key).Distinct().OrderBy(x => x).ToArray();
			var projectMaintainabilities = metricResults.ToDictionary(r => r.Item1, r => r.Item3.GroupBy(x => maintainabilityGrouping(x.MaintainabilityIndex)).ToDictionary(g => g.Key, g => g.Sum(x => x.LinesOfCode)));
			var projectSizes = metricResults.ToDictionary(r => r.Item1, r => r.Item3.Sum(x => x.LinesOfCode));
			worksheet.Cells[2, 1].Value = "Maintainability / Project";
			for (int j = 0; j < projectMaintainabilities.Keys.Count; j++)
			{
				worksheet.Cells[2, 2 + j].Value = projectMaintainabilities.Keys.ElementAt(j);
			}

			for (int j = 0; j < projectMaintainabilities.Keys.Count; j++)
			{
				var projectKey = projectMaintainabilities.Keys.ElementAt(j);
				var dictionary = projectMaintainabilities[projectKey];
				worksheet.Cells[1, 2 + j].Value = dictionary.Sum(x => x.Value);
			}

			for (int i = 0; i < maintainabilities.Length; i++)
			{
				worksheet.Cells[3 + i, 1].Value = maintainabilities[i];

				for (int j = 0; j < projectMaintainabilities.Keys.Count; j++)
				{
					var projectKey = projectMaintainabilities.Keys.ElementAt(j);
					var dictionary = projectMaintainabilities[projectKey];
					var key = maintainabilities[i];
					var cell = worksheet.Cells[3 + i, 2 + j];
					cell.Style.Numberformat.Format = "0.00%";
					cell.Value = dictionary.ContainsKey(key) ? ((double)dictionary[key] / projectSizes[projectKey]) : 0;
				}
			}

			var row = maintainabilities.Length + 5;
			worksheet.Cells[row, 1].Value = "Above 60";
			for (int j = 0; j < projectMaintainabilities.Keys.Count; j++)
			{
				var col = j + 2;
				var range = worksheet.Cells[row, col];
				range.Formula = string.Format("= SUM({0}: {1})", worksheet.Cells[3, col].Address, worksheet.Cells[4, col].Address);
				range.Style.Numberformat.Format = "0.00%";
			}

			row = maintainabilities.Length + 6;
			worksheet.Cells[row, 1].Value = "Above 40";
			for (int j = 0; j < projectMaintainabilities.Keys.Count; j++)
			{
				var col = j + 2;
				var range = worksheet.Cells[row, col];
				range.Formula = string.Format("= SUM({0}: {1})", worksheet.Cells[3, col].Address, worksheet.Cells[5, col].Address);
				range.Style.Numberformat.Format = "0.00%";
			}
		}

		private static void WriteNonWeigtedMaintainabilitySheet(ExcelWorksheet worksheet, Tuple<string, int, IEnumerable<TypeMetric>>[] metricResults)
		{
			var maintainabilityGrouping = new Func<double, string>(d =>
				{
					if (d > 80)
					{
						return "a. x > 80";
					}

					if (d > 60)
					{
						return "b. 80 >= x > 60";
					}

					if (d > 40)
					{
						return "c. 60 >= x > 40";
					}

					if (d > 20)
					{
						return "d. 40 >= x > 20";
					}

					return "e. <= 20";
				});
			var maintainabilities = metricResults.SelectMany(r => r.Item3.Select(x => x.MaintainabilityIndex)).GroupBy(maintainabilityGrouping).Select(g => g.Key).Distinct().OrderBy(x => x).ToArray();
			var projectMaintainabilities = metricResults.ToDictionary(r => r.Item1, r => r.Item3.GroupBy(x => maintainabilityGrouping(x.MaintainabilityIndex)).ToDictionary(g => g.Key, g => g.Count()));
			var projectSizes = metricResults.ToDictionary(r => r.Item1, r => r.Item3.Count());
			worksheet.Cells[2, 1].Value = "Maintainability / Project";
			for (int j = 0; j < projectMaintainabilities.Keys.Count; j++)
			{
				worksheet.Cells[2, 2 + j].Value = projectMaintainabilities.Keys.ElementAt(j);
			}

			for (int j = 0; j < projectMaintainabilities.Keys.Count; j++)
			{
				var projectKey = projectMaintainabilities.Keys.ElementAt(j);
				var dictionary = projectMaintainabilities[projectKey];
				worksheet.Cells[1, 2 + j].Value = dictionary.Sum(x => x.Value);
			}

			for (int i = 0; i < maintainabilities.Length; i++)
			{
				worksheet.Cells[3 + i, 1].Value = maintainabilities[i];

				for (int j = 0; j < projectMaintainabilities.Keys.Count; j++)
				{
					var projectKey = projectMaintainabilities.Keys.ElementAt(j);
					var dictionary = projectMaintainabilities[projectKey];
					var key = maintainabilities[i];
					var cell = worksheet.Cells[3 + i, 2 + j];
					cell.Style.Numberformat.Format = "0.00%";
					cell.Value = dictionary.ContainsKey(key) ? ((double)dictionary[key] / projectSizes[projectKey]) : 0;
				}
			}

			var row = maintainabilities.Length + 5;
			worksheet.Cells[row, 1].Value = "Above 60";
			for (int j = 0; j < projectMaintainabilities.Keys.Count; j++)
			{
				var col = j + 2;
				var range = worksheet.Cells[row, col];
				range.Formula = string.Format("= SUM({0}: {1})", worksheet.Cells[3, col].Address, worksheet.Cells[4, col].Address);
				range.Style.Numberformat.Format = "0.00%";
			}

			row = maintainabilities.Length + 6;
			worksheet.Cells[row, 1].Value = "Above 40";
			for (int j = 0; j < projectMaintainabilities.Keys.Count; j++)
			{
				var col = j + 2;
				var range = worksheet.Cells[row, col];
				range.Formula = string.Format("= SUM({0}: {1})", worksheet.Cells[3, col].Address, worksheet.Cells[5, col].Address);
				range.Style.Numberformat.Format = "0.00%";
			}
		}

		private static void WriteMetricWorksheet(ExcelWorksheet worksheet, IEnumerable<Tuple<string, int, IEnumerable<TypeMetric>>> results)
		{
			var column = 2;
			worksheet.Cells[1, 1].Value = "Project";
			worksheet.Cells[2, 1].Value = "Weighted Maintainability Index";
			worksheet.Cells[3, 1].Value = "Weighted Cyclomatic Complexity";
			worksheet.Cells[4, 1].Value = "Lines of Code";
			worksheet.Cells[5, 1].Value = "Average Class Coupling";
			worksheet.Cells[6, 1].Value = "Average Depth of Inheritance";
			worksheet.Cells[7, 1].Value = "Halstead Length";
			worksheet.Cells[8, 1].Value = "Halstead Volume";
			worksheet.Cells[9, 1].Value = "Halstead Difficulty";
			worksheet.Cells[10, 1].Value = "Halstead Effort";
			worksheet.Cells[11, 1].Value = "Weighted Halstead Difficulty";
			worksheet.Cells[12, 1].Value = "Source Lines of Code";
			worksheet.Cells[13, 1].Value = "Raw Maintainability Index";
			worksheet.Cells[14, 1].Value = "Raw Cyclomatic Complexity";

			foreach (var result in results)
			{
				Console.WriteLine(result.Item1);

				var metrics = result.Item3.ToArray();
				var linesOfCode = metrics.Sum(m => m.LinesOfCode);

				var totalMaintainability = metrics
					.Where(x => !(double.IsNaN(x.MaintainabilityIndex) || x.MaintainabilityIndex > 100))
					.Sum(r => r.MaintainabilityIndex * r.LinesOfCode);
				var maintainability = totalMaintainability / linesOfCode;
				var cyclomaticComplexityMetrics = metrics.Select(r => new Tuple<double, int>(r.CyclomaticComplexity, r.LinesOfCode))
														 .ToArray();
				var cyclomaticComplexity = cyclomaticComplexityMetrics.Sum(m => m.Item1 * m.Item2) / linesOfCode;
				var rawComplexity = metrics.Any() ? metrics.Average(m => m.CyclomaticComplexity) : 0;
				var rawMaintainability = metrics.Any() ? metrics.Average(m => m.MaintainabilityIndex) : 0;
				var couplings = metrics.Select(x => x.ClassCoupling)
									   .ToArray();
				var inheritance = metrics.Select(x => x.DepthOfInheritance)
										 .ToArray();
				worksheet.Cells[1, column].Value = result.Item1;
				worksheet.Cells[2, column].Value = maintainability;
				worksheet.Cells[3, column].Value = cyclomaticComplexity;
				worksheet.Cells[4, column].Value = linesOfCode;
				worksheet.Cells[5, column].Value = couplings.Any()
													   ? couplings.Average()
													   : 0;
				worksheet.Cells[6, column].Value = inheritance.Any()
													   ? inheritance.Average()
													   : 0;
				var memberMetrics = metrics.SelectMany(m => m.MemberMetrics)
										   .ToArray();
				worksheet.Cells[7, column].Value = memberMetrics.Sum(m => m.Halstead.GetLength());
				worksheet.Cells[8, column].Value = memberMetrics.Sum(m => m.Halstead.GetVolume());
				worksheet.Cells[9, column].Value = memberMetrics.Sum(m => m.Halstead.GetDifficulty());
				worksheet.Cells[10, column].Value = memberMetrics.Sum(m => m.Halstead.GetEffort().TotalSeconds);
				worksheet.Cells[11, column].Formula = string.Format("= {0} / {1}", worksheet.Cells[9, column].Address, metrics.Length);
				worksheet.Cells[12, column].Value = result.Item2;
				worksheet.Cells[13, column].Value = rawMaintainability;
				worksheet.Cells[14, column].Value = rawComplexity;
				worksheet.Cells[1, column, 14, column].Style.Numberformat.Format = "0";
				column++;
			}
		}
	}
}