namespace ArchiMeter.Reports
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;

	using OfficeOpenXml;

	public class NamespaceMaintainabilityDeviationReport : IReportJob
	{
		private readonly IAsyncReadOnlyRepository<TypeMaintainabilityDeviation> _maintainabilityDeviationRepository;

		public NamespaceMaintainabilityDeviationReport(IAsyncReadOnlyRepository<TypeMaintainabilityDeviation> maintainabilityDeviationRepository)
		{
			_maintainabilityDeviationRepository = maintainabilityDeviationRepository;
		}

		~NamespaceMaintainabilityDeviationReport()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}

		public async Task AddReport(ExcelPackage package, ReportConfig config)
		{
			var devations = (await _maintainabilityDeviationRepository.Query(d => d.Sigma <= -2.0));
			var projectGroups = devations.GroupBy(d => d.ProjectName).ToArray();
			var ws = package.Workbook.Worksheets.Add("NS Maintainability Dev");
			ws.Cells[1, 1].Value = "Project";
			ws.Cells[1, 2].Value = "Namespace";
			ws.Cells[1, 3].Value = "Count";
			var i = 2;
			foreach (var projectGroup in projectGroups)
			{
				var namespaceDeviations = projectGroup.GroupBy(d => d.NamespaceName).Select(g => new Tuple<string, int>(g.Key, g.Count()));
				foreach (var deviation in namespaceDeviations)
				{
					ws.Cells[i, 1].Value = projectGroup.Key;
					ws.Cells[i, 2].Value = deviation.Item1;
					ws.Cells[i, 3].Value = deviation.Item2;
					i++;
				}
			}
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
	}
}