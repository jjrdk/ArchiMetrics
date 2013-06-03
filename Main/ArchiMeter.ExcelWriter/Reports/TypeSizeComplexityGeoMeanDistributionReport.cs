namespace ArchiMeter.ReportWriter.Reports
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Common;
	using Common.Documents;
	using OfficeOpenXml;

	public class TypeSizeComplexityGeoMeanDistributionReport : IReportJob
	{
		private readonly IAsyncReadOnlyRepository<TypeSizeComplexityGeoMeanSegment> _typeSizeProvider;

		public TypeSizeComplexityGeoMeanDistributionReport(IAsyncReadOnlyRepository<TypeSizeComplexityGeoMeanSegment> typeSizeProvider)
		{
			_typeSizeProvider = typeSizeProvider;
		}

		public async Task AddReport(ExcelPackage package, ReportConfig config)
		{
			var segments = (await _typeSizeProvider.Query(config.Projects.CreateQuery<TypeSizeComplexityGeoMeanSegment>()))
				.OrderBy(x => x.GeoMean)
				.ToArray();
			var max = segments.Any() ? segments.Max(s => s.GeoMean) + 1 : 0;
			var groups = segments.GroupBy(s => s.ProjectName).ToArray();
			var ws = package.Workbook.Worksheets.Add("Type SizesComplexities");
			for (var i = 0; i < groups.Length; i++)
			{
				ws.Cells[1, i + 2].Value = groups[i].Key;
			}

			for (var i = 0; i < max; i++)
			{
				var currentRow = i + 2;
				ws.Cells[currentRow, 1].Value = i;
				for (var j = 0; j < groups.Length; j++)
				{
					var segment = groups[j].Where(s => s.GeoMean == i).Select(s => s.Count).ToArray();
					if (segment.Length > 0)
					{
						ws.Cells[currentRow, j + 2].Value = segment.Sum();
					}
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~TypeSizeComplexityGeoMeanDistributionReport()
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