// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamespaceMetric.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the INamespaceMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    using System.Collections.Generic;

    /// <summary>
	/// Defines the interface for namespace metrics.
	/// </summary>
	public interface INamespaceMetric : ICodeMetric
    {
        /// <summary>
        /// Gets the max depth of inheritance for types in the namespace.
        /// </summary>
        int ClassCoupling { get; }

        int DepthOfInheritance { get; }

        /// <summary>
        /// Gets the <see cref="ITypeMetric"/> for the types defined in the namespace.
        /// </summary>
        IEnumerable<ITypeMetric> TypeMetrics { get; }

        /// <summary>
        /// Gets the level of abstractness for the namespace.
        /// </summary>
        double Abstractness { get; }

        /// <summary>
        /// Gets the <see cref="IDocumentation"/> for the namespace.
        /// </summary>
        /// <remarks>
        /// The namespace documentation uses a convention and loads the documentation from a dummy class named [namespace name]Doc.
        ///
        /// If this class does not exist then the property will return <code>null</code>.
        /// </remarks>
        IDocumentation Documentation { get; }
    }
}