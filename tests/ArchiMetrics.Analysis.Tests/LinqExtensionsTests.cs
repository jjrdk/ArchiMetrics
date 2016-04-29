// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinqExtensionsTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LinqExtensionsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests
{
    using System.Linq;
    using Common;
    using Xunit;

    public class LinqExtensionsTests
    {
        [Fact]
        public void WhenConvertingToCollectionThenItemsAreSame()
        {
            var item = new object();
            var array = new[] { item };

            var collection = array.ToCollection();

            Assert.Same(item, collection[0]);
        }

        [Fact]
        public void WhereNotReturnsItemsNotMatchedByPredicate()
        {
            var items = new[] { 1, 2, 3 };

            var result = items.WhereNot(x => x > 3);

            Assert.Equal(items, result);
        }

        [Fact]
        public void WhenComparingWordThenWhereNotNullOrWhitespaceReturnsTrue()
        {
            var array = new[] { "hello" };

            Assert.NotEmpty(array.WhereNotNullOrWhitespace());
        }

        [Fact]
        public void DistinctByMatchesByPassedPredicate()
        {
            var items = new[]
                        {
                            new TestItem { Value = 1 },
                            new TestItem { Value = 2 },
                            new TestItem { Value = 2 }
                        };

            var distinctItems = items.DistinctBy(x => x.Value);

            Assert.Equal(2, distinctItems.Count());
        }

        private class TestItem
        {
            public int Value { get; set; }
        }
    }
}
