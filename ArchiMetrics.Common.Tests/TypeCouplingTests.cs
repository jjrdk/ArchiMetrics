namespace ArchiMetrics.Common.Tests
{
	using System.Linq;
	using ArchiMetrics.Common.Metrics;
	using NUnit.Framework;

	public class TypeCouplingTests
	{
		[Test]
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

			Assert.IsFalse(first.Equals(second));
		}

		[Test]
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

			Assert.IsTrue(first != second);
		}

		[Test]
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

			Assert.IsTrue(first < second);
		}

		[Test]
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

			Assert.IsFalse(first > second);
		}
	}
}
