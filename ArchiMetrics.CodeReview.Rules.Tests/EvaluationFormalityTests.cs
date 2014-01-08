// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationFormalityTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using System.Collections;
	using System.Linq;
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

	public static class RuleProvider
	{
		public static IEnumerable Rules
		{
			get
			{
				return AllRules.GetRules()
					.Where(x => x.GetConstructors().Any(c => c.GetParameters().Length == 0));
			}
		}
	}
}
