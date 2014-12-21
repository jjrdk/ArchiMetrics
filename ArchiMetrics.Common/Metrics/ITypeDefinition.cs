// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeDefinition.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ITypeDefinition type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
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