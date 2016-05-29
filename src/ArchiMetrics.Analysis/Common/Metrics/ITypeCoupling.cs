// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeCoupling.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ITypeCoupling type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    using System;
    using System.Collections.Generic;

    /// <summary>
	/// Defines the interface for representing type coupling information.
	/// </summary>
	public interface ITypeCoupling : IComparable<ITypeCoupling>, ITypeDefinition
	{
		/// <summary>
		/// Gets the names of the used methods.
		/// </summary>
		IEnumerable<string> UsedMethods { get; }
		
		/// <summary>
		/// Gets the names of the used properties.
		/// </summary>
		IEnumerable<string> UsedProperties { get; }

		/// <summary>
		/// Gets the names of the used events.
		/// </summary>
		IEnumerable<string> UsedEvents { get; }
	}
}