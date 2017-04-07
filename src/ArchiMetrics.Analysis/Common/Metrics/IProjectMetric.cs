// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectMetric.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IProjectMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    using System.Collections.Generic;

    public interface IProjectMetric : ICodeMetric
	{
		/// <summary>
		/// Gets the <see cref="INamespaceMetric"/> for namespaces defined in the project.
		/// </summary>
		IEnumerable<INamespaceMetric> NamespaceMetrics { get; }

		IEnumerable<string> Dependants { get; }

		/// <summary>
		/// Gets the names of the project dependencies.
		/// </summary>
		IEnumerable<string> AssemblyDependencies { get; }

		/// <summary>
		/// Gets the relational cohesion for the project.
		/// </summary>
		double RelationalCohesion { get; }

		/// <summary>
		/// Gets the efferent coupling for the project.
		/// </summary>
		/// <remarks>The efferent coupling counts the number of outgoing dependencies.</remarks>
		int EfferentCoupling { get; }

		/// <summary>
		/// Gets the afferent coupling for the project.
		/// </summary>
		/// <remarks>The afferent coupling counts the number of incoming dependencies.</remarks>
		int AfferentCoupling { get; }

		/// <summary>
		/// Gets the level of abstractness for the project.
		/// </summary>
		/// <remarks>Abstractness is the ratio of abstract classes to concrete classes.</remarks>
		double Abstractness { get; }
	}
}