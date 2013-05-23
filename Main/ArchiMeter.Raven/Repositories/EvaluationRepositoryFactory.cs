// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationRepositoryFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EvaluationRepositoryFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven.Repositories
{
	using System;

	using ArchiMeter.Common.Documents;

	using Common;

	using global::Raven.Client;

	public class EvaluationRepositoryFactory : IFactory<IDataSession<EvaluationResultDocument>>
	{
		private readonly IFactory<IAsyncDocumentSession> _asyncSessionFactory;

		public EvaluationRepositoryFactory(IFactory<IAsyncDocumentSession> asyncSessionFactory)
		{
			_asyncSessionFactory = asyncSessionFactory;
		}

		public IDataSession<EvaluationResultDocument> Create()
		{
			return new EvaluationRepository(_asyncSessionFactory);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~EvaluationRepositoryFactory()
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