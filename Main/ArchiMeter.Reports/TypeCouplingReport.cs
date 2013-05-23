// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCouplingReport.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeCouplingReport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Reports
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Threading.Tasks;

	using Common;
	using Common.Metrics;
	using OfficeOpenXml;

	public class TypeCouplingReport : IReportJob
	{
		private readonly IAsyncProvider<string, string, TypeCoupling> _typeCouplingProvider;
		private TypeCoupling[] _docs;

		public TypeCouplingReport(IAsyncProvider<string, string, TypeCoupling> typeCouplingProvider)
		{
			_typeCouplingProvider = typeCouplingProvider;
		}

		public async Task AddReport(ExcelPackage package, ReportConfig config)
		{
			Console.WriteLine("Starting type coupling report.");

			foreach (var coupling in config.Couplings)
			{
				var docs = await GetDocuments(config);
				var couplings = await GetCouplings(docs, coupling.ToLowerInvariant());
				var ws = package.Workbook.Worksheets.Add(coupling);
				WriteReport(ws, couplings);
			}

			Console.WriteLine("Ending type coupling report.");
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~TypeCouplingReport()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				// Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
				_typeCouplingProvider.Dispose();
			}
		}

		private void WriteReport(ExcelWorksheet worksheet, IEnumerable<Tuple<string, int>> couplings)
		{
			worksheet.Cells[1, 1].Value = "Type";
			worksheet.Cells[1, 2].Value = "Usage Count";
			var row = 2;
			foreach (var coupling in couplings)
			{
				worksheet.Cells[row, 1].Value = coupling.Item1;
				worksheet.Cells[row, 2].Value = coupling.Item2;
				row++;
			}
		}

		private async Task<TypeCoupling[]> GetDocuments(ReportConfig config)
		{
			if (_docs == null)
			{
				var tasks = (from project in config.Projects
							 select _typeCouplingProvider.GetAll(project.Name, project.Revision.ToString(CultureInfo.InvariantCulture)))
								 .ToArray();
				var result = await Task.WhenAll(tasks);
				_docs = result
					.SelectMany(x => x)
					.ToArray();
			}

			return _docs;
		}

		private Task<IEnumerable<Tuple<string, int>>> GetCouplings(TypeCoupling[] metricsDocuments, string query)
		{
			return Task.Factory.StartNew(() => metricsDocuments
												   .Where(c => !string.IsNullOrWhiteSpace(c.Namespace))
												   .Where(c => c.Namespace.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
												   .GroupBy(c => c.ToString())
												   .Select(g => new Tuple<string, int>(g.Key, g.Count()))
												   .ToArray()
												   .AsEnumerable());
		}
	}
}
