namespace ArchiMeter.ExcelWriter.Reports
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using Common;
	using Common.Documents;
	using OfficeOpenXml;
	using OfficeOpenXml.Drawing.Chart;

	public class SizeComplexityScatterReport : IReportJob
	{
		private readonly IAsyncReadOnlyRepository<MemberSizeComplexitySegment> _repository;

		public SizeComplexityScatterReport(IAsyncReadOnlyRepository<MemberSizeComplexitySegment> repository)
		{
			_repository = repository;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public async Task AddReport(ExcelPackage package, ReportConfig config)
		{
			/*			 
			where !namespaceMetric.Name.Contains("Tests") 
			&& !namespaceMetric.Name.Contains("UnitTest") 
			&& !namespaceMetric.Name.Contains("Fakes") 
			&& !namespaceMetric.Name.Contains("Mocks")
			
			var excluded = new[] { "Tests", "UnitTest", "Fakes", "Mocks" };
			 p =>
			excluded.Aggregate<string, Expression>(
				Expression.GreaterThan(Expression.Property(p, "CyclomaticComplexity"), Expression.Constant(1)),
				(e, s) => Expression.Property(p, "Name").DoesNotContain(s));
			 */
			var worksheet = package.Workbook.Worksheets.Add("Size Complexity Scatter");
			var charts = package.Workbook.Worksheets.Add("Size Complexity Chart");
			var maxRow = await PrintValues(worksheet, config);
			PrintCharts(charts, worksheet, maxRow, config);
		}

		private async Task<int> PrintValues(ExcelWorksheet worksheet, ReportConfig config)
		{
			Func<ParameterExpression, Expression> filter = p => Expression.GreaterThan(Expression.Property(p, "CyclomaticComplexity"), Expression.Constant(1));
			var results = (await _repository.Query(config.Projects.CreateQuery<MemberSizeComplexitySegment>(filter))).ToArray();
			var locs = results.Select(x => x.LoC).Distinct().OrderBy(x => x).ToArray();
			var projects = results
				.GroupBy(x => string.Format("{0} {1}", x.ProjectName, x.Date.ToString("yyyy-MM-dd")))
				.OrderBy(x => x.Key)
				.ToArray();
			worksheet.Cells[1, 1].Value = "Lines of Code";

			for(var i = 0; i < projects.Length; i++)
			{
				worksheet.Cells[1, i + 2].Value = projects[i].Key;
			}

			var row = 2;
			foreach(var value in locs)
			{
				int value1 = value;
				var allCCs = projects.SelectMany(x => x).Where(x => x.LoC == value1).Select(x => x.CyclomaticComplexity).Distinct().OrderBy(x => x);
				foreach(var cc in allCCs)
				{
					worksheet.Cells[row, 1].Value = value;
					for(var i = 0; i < projects.Length; i++)
					{
						if(projects[i].Any(x => x.CyclomaticComplexity == cc && x.LoC == value1))
						{
							worksheet.Cells[row, i + 2].Value = projects[i].First(x => x.CyclomaticComplexity == cc && x.LoC == value1).CyclomaticComplexity;
						}
					}
					row++;
				}
			}

			return row;
		}

		private void PrintCharts(ExcelWorksheet worksheet, ExcelWorksheet sourceSheet, int maxRow, ReportConfig config)
		{
			var x = 0;
			var y = 0;
			var projects = config.Projects.OrderBy(p => p.Name).ToArray();
			for (var i = 0; i < projects.Length; i++)
			{
				var settings = projects[i];
				var title = string.Format("{0} {1}", settings.Name, settings.Date.ToString("yyyy-MM-dd"));

				var chart = worksheet.Drawings.AddChart(title, eChartType.XYScatter);
				var col = i + 2;
				var serie = chart.Series.Add(sourceSheet.Cells[2, col, maxRow, col], sourceSheet.Cells[2, 1, maxRow, 1]);
				serie.HeaderAddress = sourceSheet.Cells[1, col];
				chart.Title.Text = title;
				chart.XAxis.Title.Text = "Lines of Code";
				chart.XAxis.MinValue = 0.0;
				chart.YAxis.Title.Text = "Complexity";
				chart.YAxis.MinValue = 0.0;
				chart.SetPosition(y, x);
				chart.SetSize(750, 500);
				chart.DisplayBlanksAs = eDisplayBlanksAs.Gap;
				if (x < 3750)
				{
					x += 750;
				}
				else
				{
					x = 0;
					y += 500;
				}
			}
		}

		~SizeComplexityScatterReport()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
			}
		}
	}
}