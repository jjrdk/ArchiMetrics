// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorSnippetRepositoryFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorSnippetRepositoryFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using Common;
	using Common.Documents;
	using global::Raven.Client;
	using Indexes;

	public class ErrorSnippetRepositoryFactory : IFactory<IReadOnlyDataSession<CodeErrors>>
	{
		private readonly IFactory<IDocumentSession> _documentSessionFactory;

		public ErrorSnippetRepositoryFactory(IFactory<IDocumentSession> documentSessionFactory)
		{
			_documentSessionFactory = documentSessionFactory;
		}

		public IReadOnlyDataSession<CodeErrors> Create()
		{
			return new ErrorSnippetRepository(_documentSessionFactory);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ErrorSnippetRepositoryFactory()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		private class ErrorSnippetRepository : IReadOnlyDataSession<CodeErrors>
		{
			private readonly IFactory<IDocumentSession> _sessionFactory;

			public ErrorSnippetRepository(IFactory<IDocumentSession> sessionFactory)
			{
				_sessionFactory = sessionFactory;
			}

			public Task<CodeErrors> Get(string id)
			{
				return Task.Factory.StartNew(() => (CodeErrors)null);
			}

			public Task<IEnumerable<CodeErrors>> GetAll(Expression<Func<CodeErrors, bool>> obj0)
			{
				using (var session = _sessionFactory.Create())
				{
					return session.Query<CodeErrors, CodeReviewIndex>()
							   .Where(obj0)
							   .ToListAsync()
							   .ContinueWith(t => t.Result.AsEnumerable());
				}
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			~ErrorSnippetRepository()
			{
				Dispose(false);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (disposing)
				{
					_sessionFactory.Dispose();
				}
			}
		}
	}
}
