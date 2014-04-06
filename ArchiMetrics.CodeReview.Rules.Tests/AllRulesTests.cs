namespace ArchiMetrics.CodeReview.Rules.Tests
{
	using NUnit.Framework;

	public class AllRulesTests
	{
		[Test]
		public void CanGetEnumeratinoOfCodeReviewTypes()
		{
			CollectionAssert.IsNotEmpty(AllRules.GetRules());
		}
	}
}
