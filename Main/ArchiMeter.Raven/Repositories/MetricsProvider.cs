// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MetricsProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven.Repositories
{
	using System;
	using System.Linq;

	using ArchiMeter.Common.Documents;

	using Common;
	using Common.Metrics;

	using Indexes;
	using global::Raven.Client;
	using global::Raven.Client.Linq;

	public sealed class MetricsProvider
	{
		private readonly Func<ProjectInventoryDocument, string[]> _filter;
		private readonly IFactory<IDocumentSession> _sessionProvider;

		public MetricsProvider(IFactory<IDocumentSession> sessionProvider, Func<ProjectInventoryDocument, string[]> filter)
		{
			_sessionProvider = sessionProvider;
			_filter = filter;
		}

		~MetricsProvider()
		{
			Dispose(false);
		}

		public MetricOverview GetMetrics(string projectName, string projectRevision)
		{
			using (var session = _sessionProvider.Create())
			{
				session.Advanced.MaxNumberOfRequestsPerSession = 10000;
				try
				{
					var project = session.Query<ProjectInventoryDocument, ProjectInventoryDocumentIndex>()
										 .FirstOrDefault(d => d.ProjectName == projectName && d.ProjectVersion == projectRevision);

					if (project == null)
					{
						return new MetricOverview(0, new TypeMetric[0]);
					}

					var names = _filter(project).ToArray();

					var documents = session
						.Query<ProjectMetricsDocument>()
						.Where(d => d.ProjectName.In(names) && d.ProjectVersion == projectRevision)
						.ToArray();

					var metrics = documents
						.Where(x => x != null)
						.Where(d => names.Contains(d.ProjectName))
						.Select(d => new
									 {
										 d.ProjectName, 
										 d.SourceLinesOfCode, 
										 Metrics = d.Metrics.SelectMany(m => m.TypeMetrics)
									 })
						.ToArray();
					var unfound = names.Except(metrics.Select(m => m.ProjectName));
					foreach (var name in unfound)
					{
						Console.WriteLine("Not found: " + projectName + ", " + name);
					}

					return new MetricOverview(metrics.Sum(m => m.SourceLinesOfCode), metrics.SelectMany(m => m.Metrics).ToArray());
				}
				catch (Exception exception)
				{
					Console.WriteLine(exception.Message);
					return new MetricOverview(0, new TypeMetric[0]);
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				_sessionProvider.Dispose();
			}
		}
	}
}