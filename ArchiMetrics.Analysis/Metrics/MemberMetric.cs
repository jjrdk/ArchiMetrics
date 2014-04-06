// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberMetric.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using ArchiMetrics.Common.Metrics;

	internal class MemberMetric : IMemberMetric
	{
		private readonly IHalsteadMetrics _halstead;

		public MemberMetric(
			string codeFile, 
			IHalsteadMetrics halstead, 
			MemberMetricKind kind, 
			int lineNumber, 
			int linesOfCode, 
			double maintainabilityIndex, 
			int cyclomaticComplexity, 
			string name, 
			IEnumerable<ITypeCoupling> classCouplings, 
			int numberOfParameters, 
			int numberOfLocalVariables, 
			int afferentCoupling)
		{
			_halstead = halstead;
			CodeFile = codeFile;
			Kind = kind;
			LineNumber = lineNumber;
			LinesOfCode = linesOfCode;
			MaintainabilityIndex = maintainabilityIndex;
			CyclomaticComplexity = cyclomaticComplexity;
			Name = name;
			ClassCouplings = classCouplings;
			NumberOfParameters = numberOfParameters;
			NumberOfLocalVariables = numberOfLocalVariables;
			AfferentCoupling = afferentCoupling;
		}

		public string CodeFile { get; private set; }

		public MemberMetricKind Kind { get; private set; }

		public int LineNumber { get; private set; }

		public int LinesOfCode { get; private set; }

		public double MaintainabilityIndex { get; private set; }

		public int CyclomaticComplexity { get; private set; }

		public string Name { get; private set; }

		public IEnumerable<ITypeCoupling> ClassCouplings { get; private set; }

		public int NumberOfParameters { get; private set; }

		public int NumberOfLocalVariables { get; private set; }

		public int AfferentCoupling { get; private set; }

		public IHalsteadMetrics GetHalsteadMetrics()
		{
			return _halstead;
		}

		public double GetVolume()
		{
			return _halstead == null
			? 0d
			: _halstead.GetVolume();
		}
	}
}
