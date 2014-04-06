// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHalsteadMetrics.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IHalsteadMetrics type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
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
