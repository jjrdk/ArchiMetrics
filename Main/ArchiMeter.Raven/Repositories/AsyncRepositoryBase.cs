namespace ArchiMeter.Raven.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using ArchiMeter.Common;
	using global::Raven.Client;
	using global::Raven.Client.Indexes;

	public abstract class AsyncRepositoryBase<TItem, TIndex> : IAsyncReadOnlyRepository<TItem>
		where TIndex : AbstractIndexCreationTask, new()
	{
		private readonly IFactory<IAsyncDocumentSession> _documentSessionFactory;

		public AsyncRepositoryBase(IFactory<IAsyncDocumentSession> documentSessionFactory)
		{
			_documentSessionFactory = documentSessionFactory;
		}

		public async Task<IEnumerable<TItem>> Query(Expression<Func<TItem, bool>> query)
		{
			using (var session = _documentSessionFactory.Create())
			{
				var allResults = new List<TItem>();
				var start = 0;
				while (true)
				{
					var current = await (session.Query<TItem, TIndex>().Where(query).Take(1024).Skip(start).ToListAsync());
					if (current.Count == 0)
					{
						break;
					}

					start += current.Count;
					allResults.AddRange(current);

				}
				return allResults;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_documentSessionFactory.Dispose();
			}
		}

		~AsyncRepositoryBase()
		{
			// Simply call Dispose(false).
			this.Dispose(false);
		}

	}
}