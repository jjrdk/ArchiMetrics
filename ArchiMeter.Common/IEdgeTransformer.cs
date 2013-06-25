// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEdgeTransformer.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IEdgeTransformer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IEdgeTransformer
	{
		Task<IEnumerable<EdgeItem>> TransformAsync(IEnumerable<EdgeItem> source);
	}
}