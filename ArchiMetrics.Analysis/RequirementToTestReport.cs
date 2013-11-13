// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequirementToTestReport.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RequirementToTestReport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class RequirementToTestReport
	{
		public RequirementToTestReport(int requirementId, IEnumerable<TestData> coveringTests)
		{
			var tests = coveringTests.ToArray();
			RequirementId = requirementId;
			CoveringTestNames = tests.Select(d => d.TestName).ToArray();
			CoveringTests = tests.Select(d => d.TestCode).ToArray();
			AssertsPerTest = tests.GroupBy(d => d.AssertCount).Select(g => new Tuple<int, int>(g.Key, g.Count())).ToArray();
		}

		public IEnumerable<Tuple<int, int>> AssertsPerTest { get; private set; }

		public IEnumerable<string> CoveringTests { get; private set; }

		public IEnumerable<string> CoveringTestNames { get; private set; }

		public int RequirementId { get; private set; }
	}
}
