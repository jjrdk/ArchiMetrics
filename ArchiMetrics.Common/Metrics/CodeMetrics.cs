// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeMetrics.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeMetrics type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	public class CodeMetrics
	{
		public int LinesOfCode { get; set; }

		public int DepthOfInheritance { get; set; }

		public int CyclomaticComplexity { get; set; }

		public double MaintainabilityIndex { get; set; }
	}
}
