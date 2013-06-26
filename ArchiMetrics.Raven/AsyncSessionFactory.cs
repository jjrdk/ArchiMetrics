// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncSessionFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the AsyncSessionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Raven
{
	using System;
	using Common;
	using global::Raven.Client;

	public class AsyncSessionFactory : IFactory<IAsyncDocumentSession>
	{
		private readonly IDocumentStore _store;

		public AsyncSessionFactory(IProvider<IDocumentStore> documentStoreProvider)
		{
			_store = documentStoreProvider.Get();
		}

		public IAsyncDocumentSession Create()
		{
			return _store.OpenAsyncSession();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~AsyncSessionFactory()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// _store.Dispose();
			}
		}
	}
}
