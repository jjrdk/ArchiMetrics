namespace ArchiMetrics.ExcelWriter.Reports
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Common;
	using Common.Documents;
	using OfficeOpenXml;

	public class TypeMaintainabilityDistributionReport : IReportJob
	{
		private readonly IAsyncReadOnlyRepository<TypeMaintainabilitySegment> _typeSizeProvider;

		public TypeMaintainabilityDistributionReport(IAsyncReadOnlyRepository<TypeMaintainabilitySegment> typeSizeProvider)
		{
			_typeSizeProvider = typeSizeProvider;
		}

		public async Task AddReport(ExcelPackage package, ReportConfig config)
		{
			var segments = Enumerable.ToArray<TypeMaintainabilitySegment>((await _typeSizeProvider.Query(config.Projects.CreateQuery<TypeMaintainabilitySegment>())));
			var max = segments.Any() ? segments.Max(s => s.MaintainabilityIndex) + 1 : 0;
			var groups = segments.GroupBy<TypeMaintainabilitySegment, string>(s => s.ProjectName).ToArray();
			var ws = package.Workbook.Worksheets.Add("Type Maintainabilities");
			for (var i = 0; i < groups.Length; i++)
			{
				ws.Cells[1, i + 2].Value = groups[i].Key;
			}

			for (var i = 0; i < max; i++)
			{
				var currentRow = i + 2;
				var value = max - i;
				ws.Cells[currentRow, 1].Value = value;
				for (int j = 0; j < groups.Length; j++)
				{
					var segment = groups[j].FirstOrDefault(s => s.MaintainabilityIndex.Equals(value));
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

		~TypeMaintainabilityDistributionReport()
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
