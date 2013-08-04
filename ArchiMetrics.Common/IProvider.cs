// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.Generic;

	public interface IProvider<out T> : IDisposable
	{
		T Get();
	}

	public interface IProvider<in TKey, out T> : IDisposable
	{
		T Get(TKey key);

		IEnumerable<T> GetAll(TKey key);
	}

	public interface IProvider<in TKey1, in TKey2, out T> : IDisposable
	{
		T Get(TKey1 key1, TKey2 key2);

		IEnumerable<T> GetAll(TKey1 key1, TKey2 key2);
	}
}
