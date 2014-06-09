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

namespace ArchiMetrics.Common
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	public interface IAsyncFactory<T> : IDisposable
	{
		Task<T> Create(CancellationToken cancellationToken);
	}

	public interface IAsyncFactory<in TParameter, T> : IDisposable
	{
		Task<T> Create(TParameter parameter, CancellationToken cancellationToken);
	}
}