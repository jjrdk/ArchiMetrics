// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorDataProviderFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorDataProviderFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Raven.Repositories
{
	using System;

	using ArchiMeter.Common.Documents;

	using Common;

	using global::Raven.Client;

	public class ErrorDataProviderFactory : IFactory<Func<ProjectInventoryDocument, string[]>, ErrorDataProvider>
	{
		private readonly IFactory<IDocumentSession> _documentSessionFactory;

		public ErrorDataProviderFactory(IFactory<IDocumentSession> documentSessionFactory)
		{
			_documentSessionFactory = documentSessionFactory;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public ErrorDataProvider Create(Func<ProjectInventoryDocument, string[]> obj0)
		{
			return new ErrorDataProvider(_documentSessionFactory, obj0);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				// Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
			}
		}

		~ErrorDataProviderFactory()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}
	}
}