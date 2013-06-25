// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSessionFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GenericSessionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Raven.Repositories
{
	using System;
	using Common;
	using global::Raven.Client;

	public class GenericSessionFactory<T> : IFactory<IDataSession<T>>
	{
		private readonly IFactory<IAsyncDocumentSession> _sessionFactory;

		public GenericSessionFactory(IFactory<IAsyncDocumentSession> sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		public IDataSession<T> Create()
		{
			return new DataSession<T>(_sessionFactory.Create());
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~GenericSessionFactory()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}
	}
}
