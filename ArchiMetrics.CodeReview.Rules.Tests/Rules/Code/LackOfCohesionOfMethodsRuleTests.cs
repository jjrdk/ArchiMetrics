namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Code
{
	using ArchiMetrics.CodeReview.Rules.Semantic;
	using NUnit.Framework;

	public sealed class LackOfCohesionOfMethodsRuleTests
	{
		private LackOfCohesionOfMethodsRuleTests()
		{
		}

		public class GivenALackOfCohesionOfMethodsRule
		{
			private LackOfCohesionOfMethodsRule _rule;

			[SetUp]
			public void Setup()
			{
				_rule = new LackOfCohesionOfMethodsRule();
			}

			[Test]
			public void CanSetThreshold()
			{
				Assert.DoesNotThrow(() => _rule.SetThreshold(1));
			}
		}
	}
}
