// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVertexRepository.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IVertexRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Structure
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IVertexRepository
	{
		Task<IEnumerable<IModelNode>> GetVertices(string solutionPath, CancellationToken cancellationToken);
	}
}
