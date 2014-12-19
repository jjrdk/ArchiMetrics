// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationFormalityTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EvaluationFormalityTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests
{
	using ArchiMetrics.Common.CodeReview;
	using NUnit.Framework;

	public class EvaluationFormalityTests
	{
		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasTitle(IEvaluation rule)
		{
			Assert.That(rule.Title, Is.Not.Null.Or.Empty);
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasSuggestion(IEvaluation rule)
		{
			Assert.That(rule.Suggestion, Is.Not.Null.Or.Empty);
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasEvaluatedKind(ISyntaxEvaluation rule)
		{
			Assert.That(rule.EvaluatedKind.ToString(), Is.Not.Null.Or.Empty);
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasImpactLevel(IEvaluation rule)
		{
			Assert.That(rule.ImpactLevel.ToString(), Is.Not.Null.Or.Empty);
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasQuality(IEvaluation rule)
		{
			Assert.That(rule.Quality.ToString(), Is.Not.Null.Or.Empty);
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasQualityAttribute(IEvaluation rule)
		{
			Assert.That(rule.QualityAttribute.ToString(), Is.Not.Null.Or.Empty);
		}
	}
}
