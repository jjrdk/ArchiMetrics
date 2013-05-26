namespace ArchiMeter.Reports
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Common;
	using Common.Documents;
	using OfficeOpenXml;

	public class TypeComplexityDistributionReport : IReportJob
	{
		private readonly IAsyncReadOnlyRepository<TypeComplexitySegment> _typeSizeProvider;

		public TypeComplexityDistributionReport(IAsyncReadOnlyRepository<TypeComplexitySegment> typeSizeProvider)
		{
			_typeSizeProvider = typeSizeProvider;
		}

		public async Task AddReport(ExcelPackage package, ReportConfig config)
		{
			var segments = (await _typeSizeProvider.Query(config.Projects.CreateQuery<TypeComplexitySegment>())).ToArray();
			var max = segments.Any() ? segments.Max(s => s.CyclomaticComplexity) + 1 : 0;
			var groups = segments.GroupBy(s => s.ProjectName).ToArray();
			var ws = package.Workbook.Worksheets.Add("Type Complexities");
			for (var i = 0; i < groups.Length; i++)
			{
				ws.Cells[1, i + 2].Value = groups[i].Key;
			}

			for (var i = 0; i < max; i++)
			{
				var currentRow = i + 2;
				ws.Cells[currentRow, 1].Value = i;
				for (int j = 0; j < groups.Length; j++)
				{
					var segment = groups[j].FirstOrDefault(s => s.CyclomaticComplexity == i);
					if (segment != null && segment.Count > 0)
					{
						ws.Cells[currentRow, j + 2].Value = segment.Count;
					}
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~TypeComplexityDistributionReport()
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