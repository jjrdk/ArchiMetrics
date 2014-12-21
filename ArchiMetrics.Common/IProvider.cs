// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	
	/// <summary>
	/// Defines the async provider interface.
	/// </summary>
	/// <typeparam name="TKey">The <see cref="Type"/> of the key.</typeparam>
	/// <typeparam name="T">The <see cref="Type"/> of the instance to provide.</typeparam>
	public interface IProvider<in TKey, out T> : IDisposable
	{
		/// <summary>
		/// Gets a consistent reference to the instance with the passed key.
		/// </summary>
		/// <param name="key">The key for the item to retrieve.</param>
		/// <returns>A consistent reference to the item which matches the passed key.</returns>
		T Get(TKey key);
	}
}
