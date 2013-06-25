// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericRepository.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GenericRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using Common;
	using global::Raven.Client;

	public abstract class GenericRepository<T> : IDataSession<T>
	{
		private readonly IndexSettings _indexSettings;
		private readonly IAsyncDocumentSession _session;

		public GenericRepository(IndexSettings indexSettings, IFactory<IAsyncDocumentSession> sessionProvider)
		{
			_indexSettings = indexSettings;
			_session = sessionProvider.Create();
		}

		public virtual Task<T> Get(string id)
		{
			return _session.LoadAsync<T>(id);
		}

		public virtual Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> query)
		{
			if (query == null)
			{
				return Task.Factory.StartNew(() => (IEnumerable<T>)new T[0]);
			}

			return GetQuery()
				.Where(query)
				.ToListAsync()
				.ContinueWith(t => t.Result.AsEnumerable());
		}

		public Task Store(object item)
		{
			return _session.StoreAsync(item);
		}

		public Task Flush()
		{
			return _session.SaveChangesAsync();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~GenericRepository()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			// get rid of unmanaged resources
			if (disposing)
			{
				_session.Dispose();
			}
		}

		private IQueryable<T> GetQuery()
		{
			return _indexSettings == null || string.IsNullOrWhiteSpace(_indexSettings.IndexName)
					   ? _session.Query<T>()
					   : _session.Query<T>(_indexSettings.IndexName, _indexSettings.IsMapReduce);
		}
	}
}
