// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestData.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TestData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System.Collections.Generic;

	public class TestData
	{
		public TestData(IEnumerable<int> requirementIds, int assertCount, string testName, string testCode)
		{
			RequirementIds = requirementIds;
			AssertCount = assertCount;
			TestCode = testCode;
			TestName = testName;
		}

		public string TestName { get; private set; }
		
		public string TestCode { get; private set; }
		
		public int AssertCount { get; private set; }
		
		public IEnumerable<int> RequirementIds { get; private set; }
	}
}
