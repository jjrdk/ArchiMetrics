// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSession.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DataSession type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Raven
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using Common;
	using global::Raven.Client;
	using global::Raven.Client.Linq;

	internal class DataSession<T> : IDataSession<T>
	{
		private readonly IAsyncDocumentSession _innerSession;

		public DataSession(IAsyncDocumentSession innerSession)
		{
			_innerSession = innerSession;
		}

		protected IAsyncDocumentSession InnerSession
		{
			get
			{
				return _innerSession;
			}
		}

		public Task<T> Get(string id)
		{
			return InnerSession.LoadAsync<T>(id);
		}

		public virtual Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> query)
		{
			return InnerSession
				.Query<T>()
				.Where(query)
				.ToListAsync()
				.ContinueWith(t => t.Result.AsEnumerable());
		}

		public Task Store(object item)
		{
			return InnerSession.StoreAsync(item);
		}

		public Task Flush()
		{
			return InnerSession.SaveChangesAsync();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~DataSession()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				// Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
				InnerSession.Dispose();
			}
		}
	}
}
