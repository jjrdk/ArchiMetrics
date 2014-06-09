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

namespace ArchiMetrics.Common.Tests
{
	using System.Linq;
	using NUnit.Framework;

	public class LinqExtensionsTests
	{
		[Test]
		public void WhenConvertingToCollectionThenItemsAreSame()
		{
			var item = new object();
			var array = new[] { item };

			var collection = array.ToCollection();

			Assert.AreSame(item, collection[0]);
		}

		[Test]
		public void WhereNotReturnsItemsNotMatchedByPredicate()
		{
			var items = new[] { 1, 2, 3 };

			var result = items.WhereNot(x => x > 3);

			CollectionAssert.AreEqual(items, result);
		}

		[Test]
		public void WhenComparingWordThenWhereNotNullOrWhitespaceReturnsTrue()
		{
			var array = new[] { "hello" };

			CollectionAssert.IsNotEmpty(array.WhereNotNullOrWhitespace());
		}

		[Test]
		public void DistinctByMatchesByPassedPredicate()
		{
			var items = new[]
						{
							new TestItem { Value = 1 }, 
							new TestItem { Value = 2 }, 
							new TestItem { Value = 2 }
						};

			var distinctItems = items.DistinctBy(x => x.Value);

			Assert.AreEqual(2, distinctItems.Count());
		}

		private class TestItem
		{
			public int Value { get; set; }
		}
	}
}
