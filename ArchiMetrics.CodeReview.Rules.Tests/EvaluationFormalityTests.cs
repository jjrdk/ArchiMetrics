namespace ArchiMetrics.CodeReview.Rules.Tests
{
	using System;
	using ArchiMetrics.Common.CodeReview;
	using NUnit.Framework;

	public class EvaluationFormalityTests
	{
		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasTitle(Type type)
		{
			var rule = GetRule(type);

			Assert.IsNotNullOrEmpty(rule.Title);
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasSuggestion(Type type)
		{
			var rule = GetRule(type);

			Assert.IsNotNullOrEmpty(rule.Suggestion);
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasEvaluatedKind(Type type)
		{
			var rule = GetRule(type);

			Assert.IsNotNullOrEmpty(rule.EvaluatedKind.ToString());
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasImpactLevel(Type type)
		{
			var rule = GetRule(type);

			Assert.IsNotNullOrEmpty(rule.ImpactLevel.ToString());
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasQuality(Type type)
		{
			var rule = GetRule(type);

			Assert.IsNotNullOrEmpty(rule.Quality.ToString());
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasQualityAttribute(Type type)
		{
			var rule = GetRule(type);

			Assert.IsNotNullOrEmpty(rule.QualityAttribute.ToString());
		}

		private IEvaluation GetRule(Type type)
		{
			var instance = Activator.CreateInstance(type);
			return (IEvaluation)instance;
		}
	}
}
