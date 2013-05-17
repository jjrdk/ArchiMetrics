namespace ArchiMeter.Raven.Tests.Indexes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;

	using ArchiMeter.Common.Documents;
	using ArchiMeter.Common.Metrics;
	using ArchiMeter.Raven.Indexes;

	using NUnit.Framework;

	public class TfsTypeSizeDistributionIndexTests : IndexTestBase<TfsMetricsDocument, TypeSizeSegment, TfsTypeSizeDistributionIndexTests.FakeTfsTypeSizeDistributionIndex>
	{
		[Test]
		public void WhenReducingThenCreatesGroupedData()
		{
			var data = new[]
				           {
					           new TfsMetricsDocument
						           {
							           MetricsDate = new DateTime(2013, 5, 16),
							           Metrics = new[]
								                     {
									                     new NamespaceMetric(
										                     0,
										                     0,
										                     0,
										                     new TypeCoupling[0],
										                     0,
										                     "testNS1",
										                     new[]
											                     {
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 10, 0, 0, 0, new TypeCoupling[0], "Type1"),
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 10, 0, 0, 0, new TypeCoupling[0], "Type2"),
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 10, 0, 0, 0, new TypeCoupling[0], "Type3"),
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 25, 0, 0, 0, new TypeCoupling[0], "Type4"),
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 25, 0, 0, 0, new TypeCoupling[0], "Type5")
											                     })
								                     }
						           },
					           new TfsMetricsDocument
						           {
							           MetricsDate = new DateTime(2013, 5, 15),
							           Metrics = new[]
								                     {
									                     new NamespaceMetric(
										                     0,
										                     0,
										                     0,
										                     new TypeCoupling[0],
										                     0,
										                     "testNS1",
										                     new[]
											                     {
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 10, 0, 0, 0, new TypeCoupling[0], "Type1"),
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 10, 0, 0, 0, new TypeCoupling[0], "Type2"),
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 10, 0, 0, 0, new TypeCoupling[0], "Type3"),
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 25, 0, 0, 0, new TypeCoupling[0], "Type4"),
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 25, 0, 0, 0, new TypeCoupling[0], "Type5")
											                     })
								                     }
						           }
				           };

			var result = PerformMapReduce(data);

			Assert.AreEqual(4, result.Length);
		}

		public class FakeTfsTypeSizeDistributionIndex : TfsTypeSizeDistributionIndex, ITestIndex<TfsMetricsDocument, TypeSizeSegment>
		{
			public Expression<Func<IEnumerable<TfsMetricsDocument>, IEnumerable>> GetMap()
			{
				return Map;
			}

			public Expression<Func<IEnumerable<TypeSizeSegment>, IEnumerable>> GetReduce()
			{
				return Reduce;
			}
		}
	}
}