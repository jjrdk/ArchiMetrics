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
