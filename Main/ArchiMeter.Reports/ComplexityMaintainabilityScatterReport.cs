namespace ArchiMeter.Reports
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using Common;
	using Common.Documents;
	using OfficeOpenXml;

	public class ComplexityMaintainabilityScatterReport : IReportJob
	{
		private readonly IAsyncReadOnlyRepository<MemberComplexityMaintainabilitySegment> _repository;

		public ComplexityMaintainabilityScatterReport(IAsyncReadOnlyRepository<MemberComplexityMaintainabilitySegment> repository)
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
			Func<ParameterExpression, Expression> filter = p => Expression.LessThan(Expression.Property(p, "MaintainabilityIndex"), Expression.Constant(100.0));
			var results = (await _repository.Query(config.Projects.CreateQuery<MemberComplexityMaintainabilitySegment>(filter))).ToArray();
			var ccs = results.Select(x => x.CyclomaticComplexity).Distinct().OrderBy(x => x).ToArray();
			var projects = results.GroupBy(x => x.ProjectName).OrderBy(x => x.Key).ToArray();
			var worksheet = package.Workbook.Worksheets.Add("Complexity Maintainability Scatter");
			worksheet.Cells[1, 1].Value = "Complexity";

			for (var i = 0; i < projects.Length; i++)
			{
				worksheet.Cells[1, i + 2].Value = projects[i].Key;
			}

			var row = 2;
			foreach (var complexity in ccs)
			{
				int complexity1 = complexity;
				var allMis = projects.SelectMany(x => x).Where(x => x.CyclomaticComplexity == complexity1).Select(x => x.MaintainabilityIndex).Distinct().OrderBy(x => x);
				foreach (var mi in allMis)
				{
					worksheet.Cells[row, 1].Value = complexity;
					for (var i = 0; i < projects.Length; i++)
					{
						if (projects[i].Any(x => x.CyclomaticComplexity == complexity1 && x.MaintainabilityIndex == mi))
						{
							worksheet.Cells[row, i + 2].Value = projects[i].First(x => x.CyclomaticComplexity == complexity1 && x.MaintainabilityIndex == mi).MaintainabilityIndex;
						}
					}
					row++;
				}
			}
		}

		~ComplexityMaintainabilityScatterReport()
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