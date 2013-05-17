// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHalsteadMetrics.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IHalsteadMetrics type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common.Metrics
{
	using System;

	public interface IHalsteadMetrics
	{
		int NumberOfOperands { get; }

		int NumberOfOperators { get; }

		int NumberOfUniqueOperands { get; }

		int NumberOfUniqueOperators { get; }

		int GetBugs();

		double GetDifficulty();

		TimeSpan GetEffort();

		int GetLength();

		int GetVocabulary();

		double GetVolume();

		IHalsteadMetrics Merge(IHalsteadMetrics metrics);
	}
}