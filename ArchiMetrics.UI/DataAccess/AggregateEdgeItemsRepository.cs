// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregateEdgeItemsRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;
	using Roslyn.Services;

	public class AggregateEdgeItemsRepository : IEdgeItemsRepository, IDisposable
	{
		private readonly ISolutionEdgeItemsRepositoryConfig _config;
		private readonly NamespaceEdgeItemsRepository _namespaceEdgeRepository;
		private readonly ProjectEdgeItemsRepository _projectEdgeRepository;

		public AggregateEdgeItemsRepository(
			ISolutionEdgeItemsRepositoryConfig config, 
			IProvider<string, ISolution> solutionProvider, 
			ICodeErrorRepository codeErrorRepository, 
			ICodeMetricsCalculator metricsCalculator)
		{
			_config = config;
			_namespaceEdgeRepository = new NamespaceEdgeItemsRepository(config, solutionProvider, codeErrorRepository);
			_projectEdgeRepository = new ProjectEdgeItemsRepository(config, solutionProvider, codeErrorRepository, metricsCalculator);
		}

		~AggregateEdgeItemsRepository()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public Task<IEnumerable<MetricsEdgeItem>> GetEdges(CancellationToken cancellationToken = default(CancellationToken))
		{
			switch (_config.Source)
			{
				case EdgeSource.Namespace:
					return _namespaceEdgeRepository.GetEdges();
				case EdgeSource.Project:
				default:
					return _projectEdgeRepository.GetEdges();
			}
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_namespaceEdgeRepository.Dispose();
				_projectEdgeRepository.Dispose();
			}
		}
	}
}
