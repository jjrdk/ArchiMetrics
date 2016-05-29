// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAvailableRules.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IAvailableRules type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using CodeReview;

    /// <summary>
	/// Defines the interface for accessing temporally available items.
	/// </summary>
	public interface IAvailableRules : IEnumerable<IEvaluation>, INotifyCollectionChanged, IDisposable
	{
		/// <summary>
		/// Gets an <see cref="IEnumerable{T}"/> of available items.
		/// </summary>
		IEnumerable<IAvailability> Availabilities { get; }
	}
}