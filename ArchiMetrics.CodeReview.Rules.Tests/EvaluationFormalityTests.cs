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
	using System;
	using ArchiMetrics.Common.CodeReview;
	using NUnit.Framework;

	public class EvaluationFormalityTests
	{
		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasTitle(Type type)
		{
			var rule = GetRule(type);

			Assert.That(rule.Title, Is.Not.Null.Or.Empty);
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasSuggestion(Type type)
		{
			var rule = GetRule(type);

			Assert.That(rule.Suggestion, Is.Not.Null.Or.Empty);
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasEvaluatedKind(Type type)
		{
			var rule = GetRule(type);

			Assert.That(rule.EvaluatedKind.ToString(), Is.Not.Null.Or.Empty);
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasImpactLevel(Type type)
		{
			var rule = GetRule(type);

			Assert.That(rule.ImpactLevel.ToString(), Is.Not.Null.Or.Empty);
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasQuality(Type type)
		{
			var rule = GetRule(type);

			Assert.That(rule.Quality.ToString(), Is.Not.Null.Or.Empty);
		}

		[TestCaseSource(typeof(RuleProvider), "Rules")]
		public void HasQualityAttribute(Type type)
		{
			var rule = GetRule(type);

			Assert.That(rule.QualityAttribute.ToString(), Is.Not.Null.Or.Empty);
		}

		private IEvaluation GetRule(Type type)
		{
			var instance = Activator.CreateInstance(type);
			return (IEvaluation)instance;
		}
	}
}
