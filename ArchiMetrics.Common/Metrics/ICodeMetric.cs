// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeMetric.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ICodeMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	public interface ICodeMetric
	{
		int LinesOfCode { get; }
		
		double MaintainabilityIndex { get; }
		
		int CyclomaticComplexity { get; }
		
		string Name { get; }
	}
}