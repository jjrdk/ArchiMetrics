// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorDataProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorDataProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Raven.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common;
	using Common.Documents;
	using Indexes;
	using global::Raven.Client;
	using global::Raven.Client.Linq;

	public sealed class ErrorDataProvider
	{
		private readonly Func<ProjectInventoryDocument, string[]> _filter;
		private readonly IFactory<IDocumentSession> _sessionProvider;

		public ErrorDataProvider(IFactory<IDocumentSession> sessionProvider, Func<ProjectInventoryDocument, string[]> filter)
		{
			_sessionProvider = sessionProvider;
			_filter = filter;
		}

		public IEnumerable<ErrorData> GetErrors(string projectName, string projectRevision)
		{
			using (var session = _sessionProvider.Create())
			{
				session.Advanced.MaxNumberOfRequestsPerSession = 10000;
				try
				{
					var project = session
						.Query<ProjectInventoryDocument, ProjectInventoryDocumentIndex>()
						.FirstOrDefault(d => d.ProjectName == projectName && d.ProjectVersion == projectRevision);

					if (project == null)
					{
						return new ErrorData[0];
					}

					var names = _filter(project).ToArray();

					return session.Query<ErrorData, CodeReviewIndex>()
								  .Where(d => d.ProjectVersion == projectRevision && d.ProjectName.In(names))
								  .ToArray()
								  .GroupBy(d => d.Error)
								  .Select(g => new ErrorData
												   {
													   DistinctLoc = g.Sum(_ => _.DistinctLoc), 
													   Effort = g.Sum(_ => _.Effort), 
													   Error = g.Key, 
													   Occurrences = g.Sum(_ => _.Occurrences), 
													   ProjectName = projectName, 
													   ProjectVersion = projectRevision
												   })
								  .ToArray();
				}
				catch (Exception exception)
				{
					Console.WriteLine(exception.Message);
					return new ErrorData[0];
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
				// get rid of managed resources
			}

			// get rid of unmanaged resources
		}

		~ErrorDataProvider()
		{
			Dispose(false);
		}
	}
}