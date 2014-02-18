// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMemberMetric.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IMemberMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	public interface IMemberMetric : ICodeMetric
	{
		MemberMetricKind Kind { get; }

		string CodeFile { get; }

		int LineNumber { get; }

		IEnumerable<ITypeCoupling> ClassCouplings { get; }

		int NumberOfParameters { get; }

		int NumberOfLocalVariables { get; }

		int AfferentCoupling { get; }

		double GetVolume();

		IHalsteadMetrics GetHalsteadMetrics();
	}
}