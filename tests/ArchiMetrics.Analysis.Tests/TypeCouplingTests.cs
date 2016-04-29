// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCouplingTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeCouplingTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests
{
    using System.Linq;
    using Common.Metrics;
    using Xunit;

    public class TypeCouplingTests
    {
        [Fact]
        public void WhenComparingTwoDifferentCouplingsWithEqualsThenReturnsFalse()
        {
            var first = new TypeCoupling(
                "classA",
                "ns",
                "assembly",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>());
            var second = new TypeCoupling(
                "classB",
                "ns",
                "assembly",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>());

            Assert.False(first.Equals(second));
        }

        [Fact]
        public void WhenComparingTwoDifferentCouplingsWithLogicalNotEqualsThenReturnsTrue()
        {
            var first = new TypeCoupling(
                "classA",
                "ns",
                "assembly",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>());
            var second = new TypeCoupling(
                "classB",
                "ns",
                "assembly",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>());

            Assert.True(first != second);
        }

        [Fact]
        public void WhenComparingTwoDifferentCouplingsThenAreOrderedAlphabetically1()
        {
            var first = new TypeCoupling(
                "classA",
                "ns",
                "assembly",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>());
            var second = new TypeCoupling(
                "classB",
                "ns",
                "assembly",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>());

            Assert.True(first < second);
        }

        [Fact]
        public void WhenComparingTwoDifferentCouplingsThenAreOrderedAlphabetically2()
        {
            var first = new TypeCoupling(
                "classA",
                "ns",
                "assembly",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>());
            var second = new TypeCoupling(
                "classB",
                "ns",
                "assembly",
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>());

            Assert.False(first > second);
        }
    }
}
