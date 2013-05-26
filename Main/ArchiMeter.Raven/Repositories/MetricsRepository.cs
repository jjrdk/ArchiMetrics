// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricsRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MetricsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven.Repositories
{
	using Common;
	using Common.Documents;
	using global::Raven.Client;

	public class MetricsRepository : GenericRepository<ProjectMetricsDocument>
	{
		public MetricsRepository(IFactory<IAsyncDocumentSession> sessionProvider)
			: base(null, sessionProvider)
		{
		}
	}
}