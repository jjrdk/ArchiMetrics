// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEdgeItemsRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IEdgeItemsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Structure
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	public interface IEdgeItemsRepository
	{
		Task<IEnumerable<MetricsEdgeItem>> GetEdges(string path, bool includeReview, CancellationToken cancellationToken);
	}
}
