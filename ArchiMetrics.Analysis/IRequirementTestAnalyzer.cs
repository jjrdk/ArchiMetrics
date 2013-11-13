// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequirementTestAnalyzer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IRequirementTestAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System.Collections.Generic;

	public interface IRequirementTestAnalyzer
	{
		IEnumerable<TestData> GetTestData(string path);

		IEnumerable<RequirementToTestReport> GetRequirementTests(string path);
	}
}
