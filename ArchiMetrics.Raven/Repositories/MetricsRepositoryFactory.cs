// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsRepositoryFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MetricsRepositoryFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Raven.Repositories
{
	using System;
	using Common;
	using Common.Documents;
	using global::Raven.Client;

	public class MetricsRepositoryFactory : IFactory<IDataSession<ProjectMetricsDocument>>
	{
		private readonly IFactory<IAsyncDocumentSession> _asyncSessionFactory;

		public MetricsRepositoryFactory(IFactory<IAsyncDocumentSession> asyncSessionFactory)
		{
			_asyncSessionFactory = asyncSessionFactory;
		}

		public IDataSession<ProjectMetricsDocument> Create()
		{
			return new MetricsRepository(_asyncSessionFactory);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~MetricsRepositoryFactory()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// get rid of managed resources
			}
		}
	}
}
