// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadOnlyDataSession.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IReadOnlyDataSession type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Threading.Tasks;

	public interface IReadOnlyDataSession<T> : IDisposable
	{
		Task<T> Get(string id);

		Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> query);
	}
}
