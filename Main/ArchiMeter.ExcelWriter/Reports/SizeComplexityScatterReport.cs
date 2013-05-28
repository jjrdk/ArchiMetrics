namespace ArchiMeter.ReportWriter.Reports
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;

	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;

	using OfficeOpenXml;

	public class SizeComplexityScatterReport : IReportJob
	{
		private readonly IAsyncReadOnlyRepository<MemberSizeComplexitySegment> _repository;

		public SizeComplexityScatterReport(IAsyncReadOnlyRepository<MemberSizeComplexitySegment> repository)
		{
			this._repository = repository;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public async Task AddReport(ExcelPackage package, ReportConfig config)
		{
			/*			 
			where !namespaceMetric.Name.Contains("Tests") 
			&& !namespaceMetric.Name.Contains("UnitTest") 
			&& !namespaceMetric.Name.Contains("Fakes") 
			&& !namespaceMetric.Name.Contains("Mocks")
			 */

			Func<ParameterExpression, Expression> filter = p => Expression.GreaterThan(Expression.Property(p, "CyclomaticComplexity"), Expression.Constant(1));
			var results = (await this._repository.Query(config.Projects.CreateQuery<MemberSizeComplexitySegment>(filter))).ToArray();
			var locs = results.Select(x => x.LoC).Distinct().OrderBy(x => x).ToArray();
			var projects = results.GroupBy(x => x.ProjectName).OrderBy(x => x.Key).ToArray();
			var worksheet = package.Workbook.Worksheets.Add("Size Complexity Scatter");
			worksheet.Cells[1, 1].Value = "Lines of Code";

			for (var i = 0; i < projects.Length; i++)
			{
				worksheet.Cells[1, i + 2].Value = projects[i].Key;
			}

			var row = 2;
			foreach (var value in locs)
			{
				int value1 = value;
				var allCCs = projects.SelectMany(x => x).Where(x => x.LoC == value1).Select(x => x.CyclomaticComplexity).Distinct().OrderBy(x => x);
				foreach (var cc in allCCs)
				{
					worksheet.Cells[row, 1].Value = value;
					for (var i = 0; i < projects.Length; i++)
					{
						if (projects[i].Any(x => x.CyclomaticComplexity == cc && x.LoC == value1))
						{
							worksheet.Cells[row, i + 2].Value = projects[i].First(x => x.CyclomaticComplexity == cc && x.LoC == value1).CyclomaticComplexity;
						}
					}
					row++;
				}
			}
		}

		~SizeComplexityScatterReport()
		{
			// Simply call Dispose(false).
			this.Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
			}
		}
	}
}