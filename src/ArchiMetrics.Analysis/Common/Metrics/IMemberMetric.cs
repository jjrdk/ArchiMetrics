// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMemberMetric.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IMemberMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    public interface IMemberMetric : ICodeMetric
	{
		/// <summary>
		/// Gets the access modifier for the member.
		/// </summary>
		AccessModifierKind AccessModifier { get; }

		/// <summary>
		/// Gets the path to the source file containing the member declaration.
		/// </summary>
		string CodeFile { get; }

		/// <summary>
		/// Gets the line number in the source file where the member is declared.
		/// </summary>
		int LineNumber { get; }

		/// <summary>
		/// Gets the number of parameters for the member.
		/// </summary>
		int ClassCoupling { get; }

		int NumberOfParameters { get; }

		/// <summary>
		/// Gets the number of local variables in the member.
		/// </summary>
		int NumberOfLocalVariables { get; }

		/// <summary>
		/// Gets the afferent coupling for the member.
		/// </summary>
		/// <remarks>Afferent coupling counts the number of incoming dependencies, i.e. number of locations the member is called.</remarks>
		int AfferentCoupling { get; }

		/// <summary>
		/// Gets the <see cref="IMemberDocumentation"/> for the member.
		/// </summary>
		IMemberDocumentation Documentation { get; }

		/// <summary>
		/// Gets the volume for the underlying source code.
		/// </summary>
		/// <returns>The volume as a <see cref="double"/>.</returns>
		double GetVolume();

		/// <summary>
		/// Gets the Halstead metrics for the member.
		/// </summary>
		/// <returns>The Halstead metrics as an <see cref="IHalsteadMetrics"/>.</returns>
		IHalsteadMetrics GetHalsteadMetrics();
	}
}