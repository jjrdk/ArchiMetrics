// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamespaceMetric.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the INamespaceMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	/// <summary>
	/// Defines the interface for namespace metrics.
	/// </summary>
	public interface INamespaceMetric : IDocumentedMetric
	{
		/// <summary>
		/// Gets the max depth of inheritance for types in the namespace.
		/// </summary>
		int DepthOfInheritance { get; }
		
		/// <summary>
		/// Gets the <see cref="ITypeMetric"/> for the types defined in the namespace.
		/// </summary>
		IEnumerable<ITypeMetric> TypeMetrics { get; }

		double Abstractness { get; }
	}
}