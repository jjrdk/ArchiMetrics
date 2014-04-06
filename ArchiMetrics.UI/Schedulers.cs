// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Schedulers.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the Schedulers type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI
{
	using System.Reactive.Concurrency;

	internal static class Schedulers
	{
		public static IScheduler Taskpool { get; set; }

		public static IScheduler Dispatcher { get; set; }
	}
}