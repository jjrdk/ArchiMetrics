// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ICodeMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;

    /// <summary>
	/// Defines the interface for a code metrics calculator for a project in a solution context.
	/// </summary>
	public interface ICodeMetricsCalculator
	{
		/// <summary>
		/// Creates a <see cref="Task{TResult}"/> which will return the metrics for the namespaces in the defined project.
		/// </summary>
		/// <param name="project">The <see cref="Project"/> to calculate metrics for.</param>
		/// <param name="solution">The <see cref="Solution"/> the project is contained in.</param>
		/// <remarks>If the <paramref name="solution"/> argument is <code>null</code>, then the project metrics are calculated for a standalone project. This may affect metrics such as afferent and efferent coupling.</remarks>
		/// <returns>A <see cref="Task{TResult}"/> providing an <see cref="IEnumerable{T}"/> of <see cref="INamespaceMetric"/> instances.</returns>
		Task<IEnumerable<INamespaceMetric>> Calculate(Project project, Solution solution);

		/// <summary>
		/// Creates a <see cref="Task{TResult}"/> which will return the metrics for the defined <see cref="SyntaxTree"/>.
		/// </summary>
		/// <param name="syntaxTrees">The <see cref="IEnumerable{SyntaxTree}"/> to calculate metrics for.</param>
		/// <param name="references">The assemblies referenced by the code snippet.</param>
		/// <returns>A <see cref="Task{TResult}"/> providing the calculated metrics as an <see cref="IEnumerable{INamespaceMetric}"/>.</returns>
		Task<IEnumerable<INamespaceMetric>> Calculate(IEnumerable<SyntaxTree> syntaxTrees, params Assembly[] references);
	}
}
