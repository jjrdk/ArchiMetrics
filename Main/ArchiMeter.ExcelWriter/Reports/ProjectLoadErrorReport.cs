// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectLoadErrorReport.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectLoadErrorReport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.ExcelWriter.Reports
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using CodeReview;
	using Common;
	using OfficeOpenXml;
	using Roslyn.Services;

	public class ProjectLoadErrorReport : IReportJob
	{
		private readonly Func<ProjectDefinition, bool> _pathFilter;
		private readonly IProvider<string, IProject> _projectProvider;

		public ProjectLoadErrorReport(IProvider<string, IProject> projectProvider)
		{
			_projectProvider = projectProvider;
			_pathFilter = ReportUtils.AllCode;
		}

		public Task AddReport(ExcelPackage package, ReportConfig config)
		{
			return Task.Factory.StartNew(() => GenerateReport(package, config));
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ProjectLoadErrorReport()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				// Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
				_projectProvider.Dispose();
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "hasDocuments", Justification = "Necessary to trigger exception.")]
		private void GenerateReport(ExcelPackage package, ReportConfig config)
		{
			Console.WriteLine("Generating Project Load Error Report");
			var exceptions = config.Projects.SelectMany(p => p.Roots.SelectMany(GetInvalidProjectPaths))
								   .Distinct()
								   .Select(path =>
											   {
												   Console.WriteLine(path);
												   IWorkspace workspace = null;
												   try
												   {
													   workspace = Workspace.LoadStandAloneProject(path, "Release", "AnyCPU");
													   var project = workspace.CurrentSolution.Projects.FirstOrDefault();
													   var hasDocuments = project == null
																			  ? null
																			  : project.HasDocuments
																					? project
																					: null;
												   }
												   catch (Exception ex)
												   {
													   return new Tuple<string, Exception>(path, ex);
												   }
												   finally
												   {
													   if (workspace != null)
													   {
														   workspace.Dispose();
													   }
												   }

												   return null;
											   })
								   .Where(e => e != null)
								   .Select((t, i) => new Tuple<int, string, Exception>(i + 2, t.Item1, t.Item2));

			var exceptionSheet = package.Workbook.Worksheets.Add("Project Load Errors - " + ReportUtils.GetMonth());
			exceptionSheet.Cells[1, 1].Value = "Path";
			exceptionSheet.Cells[1, 2].Value = "Message";
			exceptionSheet.Cells[1, 3].Value = "Stack Trace";

			foreach (var exception in exceptions)
			{
				var row = exception.Item1;
				exceptionSheet.Cells[row, 1].Value = exception.Item2;
				exceptionSheet.Cells[row, 2].Value = exception.Item3.Message;
				exceptionSheet.Cells[row, 3].Value = exception.Item3.StackTrace;
			}

			Console.WriteLine("Finished Project Load Error Report");
		}

		private IEnumerable<string> GetInvalidProjectPaths(ProjectDefinition path)
		{
			return Directory.GetFiles(path.Source, "*.csproj", SearchOption.AllDirectories)
							.Where(x => _pathFilter(new ProjectDefinition { IsTest = path.IsTest, Source = x }))
							.Select(p => _projectProvider.Get(p) == null ? p : string.Empty)
							.Where(s => !string.IsNullOrWhiteSpace(s));
		}
	}
}