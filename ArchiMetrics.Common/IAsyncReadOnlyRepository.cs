// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncReadOnlyRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IAsyncReadOnlyRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Threading.Tasks;

	public interface IAsyncReadOnlyRepository<T> : IDisposable
	{
		Task<IEnumerable<T>> Query(Expression<Func<T, bool>> query);
	}
}
