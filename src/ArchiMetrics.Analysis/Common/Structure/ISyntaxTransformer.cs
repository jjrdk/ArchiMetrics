// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyntaxTransformer.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ISyntaxTransformer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Structure
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISyntaxTransformer
	{
		Task<IEnumerable<IModelNode>> Transform(IEnumerable<IModelNode> source, IEnumerable<TransformRule> rules, CancellationToken cancellationToken);
	}
}