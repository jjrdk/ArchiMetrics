// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeDefinition.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ITypeDefinition type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    using System;

    /// <summary>
	/// Defines the interface for representing <see cref="Type"/> information.
	/// </summary>
	public interface ITypeDefinition : IComparable<ITypeDefinition>
	{
		/// <summary>
		/// Gets the name of the type.
		/// </summary>
		/// <remarks>This is the simple name of the type. The fully qualified name can be obtained using the information contained in the <see cref="ITypeDefinition"/>.</remarks>
		string TypeName { get; }

		/// <summary>
		/// Gets the containing namespace for the type.
		/// </summary>
		string Namespace { get; }

		/// <summary>
		/// Get the containing assembly name for the type.
		/// </summary>
		string Assembly { get; }
	}
}