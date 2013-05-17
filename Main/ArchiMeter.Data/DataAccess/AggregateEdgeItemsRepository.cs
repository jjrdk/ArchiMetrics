// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregateEdgeItemsRepository.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the AggregateEdgeItemsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Data.DataAccess
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Common;
	using Common.Metrics;
	using Roslyn.Services;

	public class AggregateEdgeItemsRepository : IEdgeItemsRepository, IDisposable
	{
		private readonly ISolutionEdgeItemsRepositoryConfig _config;
		private readonly IProjectMetricsCalculator _metricsCalculator;
		private readonly NamespaceEdgeItemsRepository _namespaceEdgeRepository;
		private readonly ProjectEdgeItemsRepository _projectEdgeRepository;

		public AggregateEdgeItemsRepository(
			ISolutionEdgeItemsRepositoryConfig config, 
			IProvider<ISolution, string> solutionProvider, 
			ICodeErrorRepository codeErrorRepository, 
			IProjectMetricsCalculator metricsCalculator)
		{
			_config = config;
			_metricsCalculator = metricsCalculator;
			_namespaceEdgeRepository = new NamespaceEdgeItemsRepository(config, solutionProvider, codeErrorRepository);
			_projectEdgeRepository = new ProjectEdgeItemsRepository(config, solutionProvider, codeErrorRepository, _metricsCalculator);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public Task<IEnumerable<EdgeItem>> GetEdgesAsync()
		{
			switch (_config.Source)
			{
				case EdgeSource.Namespace:
					return _namespaceEdgeRepository.GetEdgesAsync();
				case EdgeSource.Project:
				default:
					return _projectEdgeRepository.GetEdgesAsync();
			}
		}

		~AggregateEdgeItemsRepository()
		{
			// Simply call Dispose(false).
			Dispose(false);
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