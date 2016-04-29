// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IAsyncFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
	/// Defines the async factory interface.
	/// </summary>
	/// <remarks>The factory will return a new instance of <typeparamref name="T"/> when the Create method is called.</remarks>
	/// <typeparam name="T">The <see cref="Type"/> of the object the factory creates.</typeparam>
	/// <typeparam name="TParameter">The <see cref="Type"/> of the memberSymbol to pass to the creation method.</typeparam>
	/// <returns>A <see cref="Task{T}"/> which provides the object when the creation has finished.</returns>
	public interface IAsyncFactory<in TParameter, T> : IDisposable
	{
		/// <summary>
		/// Creates the requested instance as an asynchronous operation.
		/// </summary>
		/// <param name="memberSymbol">The memberSymbol to pass to the object creation.</param>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> to use for cancelling the object creation.</param>
		/// <returns>Returns a <see cref="Task{T}"/> which represents the instance creation task.</returns>
		Task<T> Create(TParameter memberSymbol, CancellationToken cancellationToken);
	}
}