// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common
{
	using System;

	public interface IFactory<out T> : IDisposable
	{
		T Create();
	}

	public interface IFactory<in TParameter, out T> : IDisposable
	{
		T Create(TParameter parameter);
	}

	public interface IFactory<in TParameter1, in TParameter2, out T> : IDisposable
	{
		T Create(TParameter1 parameter1, TParameter2 parameter2);
	}
}
