// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BranchModelRuleTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the BranchModelRuleTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Validation
{
    using System.Linq;
    using System.Threading.Tasks;
    using ArchiMetrics.Analysis.Model;
    using ArchiMetrics.Analysis.Validation;
    using Common.CodeReview;
    using Xunit;

    public sealed class BranchModelRuleTests
    {
        private BranchModelRuleTests()
        {
        }

        public class GivenABranchModelRule
        {
            [Fact]
            public async Task WhenTreeContainsBranchThenIsPassed()
            {
                var rule = new BranchModelRule(new ModelNode("leaf", "type", CodeQuality.Good, 0, 0, 0));
                var tree = new ModelNode("node", "type", CodeQuality.Good, 0, 0, 0, new[] { new ModelNode("leaf", "type", CodeQuality.Good, 0, 0, 0) });

                var result = await rule.Validate(tree);

                Assert.True(result.All(x => x.Passed));
            }

            [Fact]
            public async Task WhenTreeContainsComplexBranchThenIsPassed()
            {
                var rule = new BranchModelRule(new ModelNode("child", "type", CodeQuality.Good, 0, 0, 0, new[] { new ModelNode("leaf", "type", CodeQuality.Good, 0, 0, 0) }));
                var tree = new ModelNode("node", "type", CodeQuality.Good, 0, 0, 0, new[] { new ModelNode("child", "type", CodeQuality.Good, 0, 0, 0, new[] { new ModelNode("leaf", "type", CodeQuality.Good, 0, 0, 0) }) });

                var result = await rule.Validate(tree);

                Assert.True(result.All(x => x.Passed));
            }

            [Fact]
            public async Task WhenTreeDoesNotContainBranchThenIsNotPassed()
            {
                var rule = new BranchModelRule(new ModelNode("cheese", "type", CodeQuality.Good, 0, 0, 0));
                var tree = new ModelNode("node", "type", CodeQuality.Good, 0, 0, 0, new[] { new ModelNode("child", "type", CodeQuality.Good, 0, 0, 0, new[] { new ModelNode("leaf", "type", CodeQuality.Good, 0, 0, 0) }) });

                var result = await rule.Validate(tree);

                Assert.False(result.Any(x => x.Passed));
            }
        }
    }
}
