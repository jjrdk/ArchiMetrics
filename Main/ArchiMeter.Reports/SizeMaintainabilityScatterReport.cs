namespace ArchiMeter.Reports
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;
	using OfficeOpenXml;

	public class SizeMaintainabilityScatterReport : IReportJob
	{
		private readonly IAsyncReadOnlyRepository<MemberSizeMaintainabilitySegment> _repository;

		public SizeMaintainabilityScatterReport(IAsyncReadOnlyRepository<MemberSizeMaintainabilitySegment> repository)
		{
			_repository = repository;
		}

		~SizeMaintainabilityScatterReport()
		{
			// Simply call Dispose(false).
			this.Dispose(false);
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
				//Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.

			}
		}

		public async Task AddReport(ExcelPackage package, ReportConfig config)
		{
			Func<ParameterExpression, Expression> filter = p => Expression.LessThan(Expression.Property(p, "MaintainabilityIndex"), Expression.Constant(100.0));
			var results = (await _repository.Query(config.Projects.CreateQuery<MemberSizeMaintainabilitySegment>(filter))).ToArray();
			var locs = results.Select(x => x.LoC).Distinct().OrderBy(x => x).ToArray();
			var projects = results.GroupBy(x => x.ProjectName).ToArray();
			var worksheet = package.Workbook.Worksheets.Add("Size Maintainability Scatter");
			worksheet.Cells[1, 1].Value = "Lines of Code";

			for (var i = 0; i < projects.Length; i++)
			{
				worksheet.Cells[1, i + 2].Value = projects[i].Key;
			}

			var row = 2;
			foreach (var value in locs)
			{
				int value1 = value;
				var allMIs = projects.SelectMany(x => x).Where(x => x.LoC == value1).Select(x => x.MaintainabilityIndex).Distinct().OrderBy(x => x);
				foreach (var mi in allMIs)
				{
					worksheet.Cells[row, 1].Value = value;
					for (var i = 0; i < projects.Length; i++)
					{
						if (projects[i].Any(x => x.MaintainabilityIndex == mi && x.LoC == value1))
						{
							worksheet.Cells[row, i + 2].Value = projects[i].First(x => x.MaintainabilityIndex == mi && x.LoC == value1).MaintainabilityIndex;
						}
					}
					row++;
				}
			}
		}
	}
}