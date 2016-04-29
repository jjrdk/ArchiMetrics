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
    using Analysis.Common.CodeReview;
    using Xunit;

    public class EvaluationFormalityTests
    {
        [ClassData(typeof(RuleProvider))]
        public void HasTitle(IEvaluation rule)
        {
            Assert.False(string.IsNullOrEmpty(rule.Title));
        }

        [ClassData(typeof(RuleProvider))]
        public void HasSuggestion(IEvaluation rule)
        {
            Assert.False(string.IsNullOrEmpty(rule.Suggestion));
        }

        [ClassData(typeof(RuleProvider))]
        public void HasEvaluatedKind(ISyntaxEvaluation rule)
        {
            Assert.False(string.IsNullOrEmpty(rule.EvaluatedKind.ToString()));
        }

        [ClassData(typeof(RuleProvider))]
        public void HasImpactLevel(IEvaluation rule)
        {
            Assert.False(string.IsNullOrEmpty(rule.ImpactLevel.ToString()));
        }

        [ClassData(typeof(RuleProvider))]
        public void HasQuality(IEvaluation rule)
        {
            Assert.False(string.IsNullOrEmpty(rule.Quality.ToString()));
        }

        [ClassData(typeof(RuleProvider))]
        public void HasQualityAttribute(IEvaluation rule)
        {
            Assert.False(string.IsNullOrEmpty(rule.QualityAttribute.ToString()));
        }
    }
}
