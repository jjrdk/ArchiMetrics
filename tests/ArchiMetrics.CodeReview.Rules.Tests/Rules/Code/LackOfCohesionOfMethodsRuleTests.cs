// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LackOfCohesionOfMethodsRuleTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LackOfCohesionOfMethodsRuleTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Code
{
    using ArchiMetrics.CodeReview.Rules.Semantic;
    using Xunit;

    public sealed class LackOfCohesionOfMethodsRuleTests
    {
        private LackOfCohesionOfMethodsRuleTests()
        {
        }

        public class GivenALackOfCohesionOfMethodsRule
        {
            private readonly LackOfCohesionOfMethodsRule _rule;

            public GivenALackOfCohesionOfMethodsRule()
            {
                _rule = new LackOfCohesionOfMethodsRule();
            }

            [Fact]
            public void CanSetThreshold()
            {
                _rule.SetThreshold(1);
            }
        }
    }
}
