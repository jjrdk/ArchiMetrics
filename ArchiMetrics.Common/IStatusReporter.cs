// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatusReporter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IStatusReporter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System;

	public interface IStatusReporter
	{
		IObservable<IStatusMessage> Messages { get; }
	}
}