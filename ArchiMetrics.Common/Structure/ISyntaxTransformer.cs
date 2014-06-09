// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyntaxTransformer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ISyntaxTransformer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Structure
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	public interface ISyntaxTransformer
	{
		Task<IEnumerable<IModelNode>> Transform(IEnumerable<IModelNode> source, IEnumerable<TransformRule> rules, CancellationToken cancellationToken);
	}
}