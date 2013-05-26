// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsProviderFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MetricsProviderFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Raven.Repositories
{
	using System;
	using Common;
	using Common.Documents;
	using global::Raven.Client;

	public class MetricsProviderFactory : IFactory<Func<ProjectInventoryDocument, string[]>, MetricsProvider>
	{
		private readonly IFactory<IDocumentSession> _sessionProvider;

		public MetricsProviderFactory(IFactory<IDocumentSession> sessionProvider)
		{
			_sessionProvider = sessionProvider;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public MetricsProvider Create(Func<ProjectInventoryDocument, string[]> filter)
		{
			return new MetricsProvider(_sessionProvider, filter);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				// Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
				_sessionProvider.Dispose();
			}
		}

		~MetricsProviderFactory()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}

	}
}