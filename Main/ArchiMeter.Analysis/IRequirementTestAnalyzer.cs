namespace ArchiMeter.Analysis
{
	using System.Collections.Generic;

	public interface IRequirementTestAnalyzer
	{
		IEnumerable<TestData> GetTestData(string path);

		IEnumerable<RequirementToTestReport> GetRequirementTests(string path);
	}
}
