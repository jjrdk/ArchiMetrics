// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMetricsLoader.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectMetricsLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.DataLoader
{
	using System;
	using System.Globalization;
	using System.Linq;
	using System.Threading.Tasks;

	using ArchiMeter.Analysis;
	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;
	using ArchiMeter.Common.Metrics;

	using Roslyn.Services;

	public class ProjectMetricsLoader : IDataLoader
	{
		private readonly SLoCCounter _counter = new SLoCCounter();
		private readonly IProjectMetricsCalculator _metricsCalculator;
		private readonly IProvider<string, IProject> _projectProvider;
		private readonly IFactory<IDataSession<ProjectMetricsDocument>> _sessionProvider;

		public ProjectMetricsLoader(
			IProjectMetricsCalculator metricsCalculator,
			IProvider<string, IProject> projectProvider, 
			IFactory<IDataSession<ProjectMetricsDocument>> sessionProvider)
		{
			this._metricsCalculator = metricsCalculator;
			this._projectProvider = projectProvider;
			this._sessionProvider = sessionProvider;
		}

		public async Task Load(ProjectSettings settings)
		{
			Console.WriteLine("Starting Metrics loading for " + settings.Name);

			var projects = (from root in settings.Roots
							from p in this._projectProvider.GetAll(root.Source)
							select new
									   {
										   ProjectName = p.Name, 
										   MetricsTask = this._metricsCalculator.Calculate(p), 
										   SLoC = this._counter.Count(new[] { p })
									   })
				.ToArray();

			await Task.WhenAll(projects.Select(t => t.MetricsTask));

			var failures = projects.Where(x => x.MetricsTask.Exception != null)
								   .Select(x => x.MetricsTask.Exception)
								   .SelectMany(x => x.InnerExceptions);
			foreach (var failure in failures)
			{
				Console.WriteLine(failure.Message);
				Console.WriteLine(failure.StackTrace);
			}

			var docs = projects
				.Where(x => x.MetricsTask.Exception == null)
				.Select(x => new { x.ProjectName, x.SLoC, Metrics = x.MetricsTask.Result.ToArray() })
				.Select(x => new ProjectMetricsDocument
								 {
									 Id = string.Format("Metrics.{0}.v{1}", x.ProjectName, settings.Revision), 
									 SourceLinesOfCode = x.SLoC, 
									 Metrics = x.Metrics, 
									 ProjectName = x.ProjectName, 
									 ProjectVersion = settings.Revision.ToString(CultureInfo.InvariantCulture)
								 })
				.GroupBy(x => x.Id)
				.Select(g => g.First());

			using (var session = this._sessionProvider.Create())
			{
				foreach (var doc in docs)
				{
					try
					{
						await session.Store(doc);
						Console.WriteLine("Stored metrics for " + doc.ProjectName);
					}
					catch (Exception exception)
					{
						Console.WriteLine("Failed to store metrics for " + doc.ProjectName);
						Console.WriteLine(exception.Message);
						Console.WriteLine(exception.StackTrace);
					}
				}

				await session.Flush();
			}

			Console.WriteLine("Finished loading metrics for " + settings.Name);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ProjectMetricsLoader()
		{
			this.Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this._projectProvider.Dispose();
				this._sessionProvider.Dispose();
			}
		}
	}
}