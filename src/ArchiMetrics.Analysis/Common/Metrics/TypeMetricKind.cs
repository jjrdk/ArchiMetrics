// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMetricKind.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeMetricKind type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    using System;

    /// <summary>
	/// Defines the kind of <see cref="Type"/>.
	/// </summary>
	public enum TypeMetricKind
	{
		/// <summary>
		/// The type kind cannot be determined.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// The type is a class.
		/// </summary>
		Class = 1,

		/// <summary>
		/// The type is a delegate.
		/// </summary>
		Delegate = 2,

		/// <summary>
		/// The type is an interface.
		/// </summary>
		Interface = 3,

		/// <summary>
		/// The type is a struct.
		/// </summary>
		Struct = 4,
	}
}
