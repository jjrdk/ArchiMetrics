// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregateEdgeItemsRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the AggregateEdgeItemsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.UI.DataAccess
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;

	using Roslyn.Services;

	public class AggregateEdgeItemsRepository : IEdgeItemsRepository, IDisposable
	{
		private readonly ISolutionEdgeItemsRepositoryConfig _config;
		private readonly ICodeMetricsCalculator _metricsCalculator;
		private readonly NamespaceEdgeItemsRepository _namespaceEdgeRepository;
		private readonly ProjectEdgeItemsRepository _projectEdgeRepository;

		public AggregateEdgeItemsRepository(
			ISolutionEdgeItemsRepositoryConfig config,
			IProvider<string, ISolution> solutionProvider, 
			ICodeErrorRepository codeErrorRepository, 
			ICodeMetricsCalculator metricsCalculator)
		{
			this._config = config;
			this._metricsCalculator = metricsCalculator;
			this._namespaceEdgeRepository = new NamespaceEdgeItemsRepository(config, solutionProvider, codeErrorRepository);
			this._projectEdgeRepository = new ProjectEdgeItemsRepository(config, solutionProvider, codeErrorRepository, this._metricsCalculator);
		}

		~AggregateEdgeItemsRepository()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public Task<IEnumerable<EdgeItem>> GetEdgesAsync()
		{
			switch (this._config.Source)
			{
				case EdgeSource.Namespace:
					return this._namespaceEdgeRepository.GetEdgesAsync();
				case EdgeSource.Project:
				default:
					return this._projectEdgeRepository.GetEdgesAsync();
			}
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this._namespaceEdgeRepository.Dispose();
				this._projectEdgeRepository.Dispose();
			}
		}
	}
}
