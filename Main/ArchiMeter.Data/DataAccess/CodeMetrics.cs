// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeMetrics.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeMetrics type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Data.DataAccess
{
	using System.Collections.Generic;
	using Common.Metrics;

	public class CodeMetrics
	{
		public string Project { get; set; }

		public string ProjectPath { get; set; }

		public string Version { get; set; }

		public IEnumerable<NamespaceMetric> Metrics { get; set; }

		public int LinesOfCode { get; set; }

		public int DepthOfInheritance { get; set; }

		public int CyclomaticComplexity { get; set; }

		public double MaintainabilityIndex { get; set; }
	}
}