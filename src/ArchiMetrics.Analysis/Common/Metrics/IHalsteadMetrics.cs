// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHalsteadMetrics.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IHalsteadMetrics type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    using System;

    public interface IHalsteadMetrics
	{
		/// <summary>
		/// Gets the number of operands.
		/// </summary>
		int NumberOfOperands { get; }

		/// <summary>
		/// Gets the number of operators.
		/// </summary>
		int NumberOfOperators { get; }

		/// <summary>
		/// Gets the number of unique operands.
		/// </summary>
		int NumberOfUniqueOperands { get; }

		/// <summary>
		/// Gets the number of unique operators.
		/// </summary>
		int NumberOfUniqueOperators { get; }

		/// <summary>
		/// Gets the number of expected bugs in the underlying source code.
		/// </summary>
		/// <returns>The expected number of bugs as an <see cref="int"/>.</returns>
		int GetBugs();

		/// <summary>
		/// Gets the difficulty of the underlying source code.
		/// </summary>
		/// <returns>The calculated difficulty of the underlying source code as a <see cref="double"/> value.</returns>
		double GetDifficulty();

		/// <summary>
		/// Gets the estimated time to write the underlying source code.
		/// </summary>
		/// <returns>The estimated time as a <see cref="TimeSpan"/>.</returns>
		TimeSpan GetEffort();

		/// <summary>
		/// Gets the length of the underlying souce code.
		/// </summary>
		/// <returns>The length as an <see cref="int"/>.</returns>
		int GetLength();

		/// <summary>
		/// Gets the size of vocabulary of the underlying source code.
		/// </summary>
		/// <returns>The vocabulary size as an <see cref="int"/>.</returns>
		int GetVocabulary();

		/// <summary>
		/// Gets the volume of the underlying source code.
		/// </summary>
		/// <returns>The volume as a <see cref="double"/>.</returns>
		double GetVolume();

		/// <summary>
		/// Creates a new instance of an <see cref="IHalsteadMetrics"/> by merging another instance into the current.
		/// </summary>
		/// <param name="metrics">The other instance to merge.</param>
		/// <returns>The new <see cref="IHalsteadMetrics"/> instance from the merged sources.</returns>
		IHalsteadMetrics Merge(IHalsteadMetrics metrics);
	}
}
