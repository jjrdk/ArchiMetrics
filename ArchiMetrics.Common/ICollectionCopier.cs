// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICollectionCopier.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ICollectionCopier type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface ICollectionCopier
	{
		Task<IEnumerable<T>> Copy<T>(IEnumerable<T> source);
	}
}
