// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceMetric.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common.Metrics
{
	using System.Collections.Generic;

	public class NamespaceMetric
	{
		public NamespaceMetric(
			double maintainabilityIndex, 
			int cyclomaticComplexity, 
			int linesOfCode, 
			IEnumerable<TypeCoupling> classCouplings, 
			int depthOfInheritance, 
			string name, 
			IEnumerable<TypeMetric> typeMetrics)
		{
			MaintainabilityIndex = maintainabilityIndex;
			CyclomaticComplexity = cyclomaticComplexity;
			LinesOfCode = linesOfCode;
			ClassCouplings = classCouplings;
			DepthOfInheritance = depthOfInheritance;
			Name = name;
			TypeMetrics = typeMetrics;
		}
	
		public double MaintainabilityIndex { get; set; }
		
		public int CyclomaticComplexity { get; set; }
		
		public int LinesOfCode { get; set; }
		
		public IEnumerable<TypeCoupling> ClassCouplings { get; set; }
		
		public int DepthOfInheritance { get; set; }
		
		public string Name { get; set; }
		
		public IEnumerable<TypeMetric> TypeMetrics { get; set; }
	}
}