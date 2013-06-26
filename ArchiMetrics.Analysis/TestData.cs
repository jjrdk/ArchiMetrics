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
