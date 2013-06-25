// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberMetric.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	public class MemberMetric
	{
		public MemberMetric(
			string codeFile, 
			IHalsteadMetrics halstead, 
			MemberMetricKind kind, 
			int lineNumber, 
			int linesOfCode, 
			double maintainabilityIndex, 
			int cyclomaticComplexity, 
			string name, 
			int logicalComplexity, 
			IEnumerable<TypeCoupling> classCouplings, 
			int numberOfParameters, 
			int numberOfLocalVariables)
		{
			CodeFile = codeFile;
			Halstead = halstead;
			Kind = kind;
			LineNumber = lineNumber;
			LinesOfCode = linesOfCode;
			MaintainabilityIndex = maintainabilityIndex;
			CyclomaticComplexity = cyclomaticComplexity;
			Name = name;
			LogicalComplexity = logicalComplexity;
			ClassCouplings = classCouplings;
			NumberOfParameters = numberOfParameters;
			NumberOfLocalVariables = numberOfLocalVariables;
		}

		public string CodeFile { get; set; }

		public IHalsteadMetrics Halstead { get; set; }

		public MemberMetricKind Kind { get; set; }

		public int LineNumber { get; set; }

		public int LinesOfCode { get; set; }

		public double MaintainabilityIndex { get; set; }

		public int CyclomaticComplexity { get; set; }

		public string Name { get; set; }

		public int LogicalComplexity { get; set; }

		public IEnumerable<TypeCoupling> ClassCouplings { get; set; }

		public int NumberOfParameters { get; set; }

		public int NumberOfLocalVariables { get; set; }
	}
}
