// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncProvider.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IAsyncProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Threading.Tasks;

	public interface IAsyncProvider<T> : IDisposable
	{
		Task<T> Get();
	}

	public interface IAsyncProvider<in TKey1, in TKey2, T> : IDisposable
	{
		Task<T> Get(TKey1 key1, TKey2 key2);

		Task<IEnumerable<T>> GetAll(TKey1 key1, TKey2 key2);
	}

	public interface IAsyncReadOnlyRepository<T> : IDisposable
	{
		Task<IEnumerable<T>> Query(Expression<Func<T, bool>> query);
	}
}