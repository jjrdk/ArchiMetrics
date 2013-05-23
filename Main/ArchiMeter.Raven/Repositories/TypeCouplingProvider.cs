// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCouplingProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeCouplingProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Raven.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using ArchiMeter.Common.Documents;

	using Common;
	using Common.Metrics;

	using global::Raven.Client;
	using global::Raven.Client.Linq;

	public class TypeCouplingProvider : IAsyncProvider<string, string, TypeCoupling>
	{
		private readonly IFactory<IAsyncDocumentSession> _sessionProvider;

		public TypeCouplingProvider(IFactory<IAsyncDocumentSession> sessionProvider)
		{
			_sessionProvider = sessionProvider;
		}

		public Task<TypeCoupling> Get(string key1, string key2)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<TypeCoupling>> GetAll(string projectName, string projectVersion)
		{
			using (var session = _sessionProvider.Create())
			{
				session.Advanced.MaxNumberOfRequestsPerSession = 10000;
				try
				{
					var projects = await session.Query<ProjectInventoryDocument>()
										  .Where(d => d.ProjectName == projectName && d.ProjectVersion == projectVersion)
										  .ToListAsync();

					var names = projects.Select(d => d.ProductionProjectNames)
										.SelectMany(x => x)
										.ToArray();

					var listAsync = await session.Query<ProjectMetricsDocument>()
										   .Where(d => d.ProjectName.In(names) && d.ProjectVersion == projectVersion)
										   .ToListAsync();
					var namespaceMetrics = listAsync
						.SelectMany(d => d.Metrics)
						.ToArray();
					var namespaceCouplings = namespaceMetrics
						.SelectMany(x => x.ClassCouplings)
						.Where(c => !c.Namespace.Contains(projectName));
					var typeCouplings = namespaceMetrics
						.SelectMany(d => d.TypeMetrics)
						.SelectMany(d => d.ClassCouplings)
						.Where(c => !c.Namespace.Contains(projectName));
					var methodCouplings = namespaceMetrics
						.SelectMany(x => x.TypeMetrics)
						.SelectMany(x => x.MemberMetrics)
						.SelectMany(x => x.ClassCouplings)
						.Where(c => !c.Namespace.Contains(projectName));

					return namespaceCouplings.Concat(typeCouplings).Concat(methodCouplings).ToArray();
				}
				catch (Exception exception)
				{
					Console.WriteLine(exception.Message);
					return new TypeCoupling[0];
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// get rid of managed resources
			}

			// get rid of unmanaged resources
		}

		~TypeCouplingProvider()
		{
			Dispose(false);
		}
	}
}