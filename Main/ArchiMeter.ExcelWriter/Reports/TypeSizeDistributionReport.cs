namespace ArchiMeter.ReportWriter.Reports
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;

	using OfficeOpenXml;

	public class TypeSizeDistributionReport : IReportJob
	{
		private readonly IAsyncReadOnlyRepository<TypeSizeSegment> _typeSizeProvider;

		public TypeSizeDistributionReport(IAsyncReadOnlyRepository<TypeSizeSegment> typeSizeProvider)
		{
			this._typeSizeProvider = typeSizeProvider;
		}

		public async Task AddReport(ExcelPackage package, ReportConfig config)
		{
			var segments = (await this._typeSizeProvider.Query(config.Projects.CreateQuery<TypeSizeSegment>())).ToArray();
			var max = segments.Any() ? segments.Max(s => s.LoC) + 1 : 0;
			var groups = segments.GroupBy(s => s.ProjectName).ToArray();
			var ws = package.Workbook.Worksheets.Add("Type Sizes");
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
					var segment = groups[j].FirstOrDefault(s => s.LoC == i);
					if (segment != null && segment.Count > 0)
					{
						ws.Cells[currentRow, j + 2].Value = segment.Count;
					}
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~TypeSizeDistributionReport()
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