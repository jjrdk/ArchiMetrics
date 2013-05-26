namespace ArchiMeter.Raven.Tests.Indexes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Common.Documents;
	using Common.Metrics;
	using NUnit.Framework;
	using Raven.Indexes;

	public class TfsWeightedProjectMetricsIndexTests : IndexTestBase<TfsMetricsDocument, TfsProjectMetrics, TfsWeightedProjectMetricsIndexTests.FakeTfsWeightedProjectMetricsIndex>
	{
		[Test]
		public void WhenReducingThenCreatesGroupedData()
		{
			var data = new[]
				           {
					           new TfsMetricsDocument
						           {
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
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 20, 0, 0, 0, new TypeCoupling[0], "Type2"),
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 30, 0, 0, 0, new TypeCoupling[0], "Type3"),
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 15, 0, 0, 0, new TypeCoupling[0], "Type4"),
												                     new TypeMetric(TypeMetricKind.Unknown, new MemberMetric[0], 25, 0, 0, 0, new TypeCoupling[0], "Type5")
											                     }),
								                     }
						           }
				           };

			var result = PerformMapReduce(data);

			Assert.AreEqual(1, result.Length);
		}

		public class FakeTfsWeightedProjectMetricsIndex : TfsWeightedProjectMetricsIndex, ITestIndex<TfsMetricsDocument, TfsProjectMetrics>
		{
			public Expression<Func<IEnumerable<TfsMetricsDocument>, IEnumerable>> GetMap()
			{
				return Map;
			}

			public Expression<Func<IEnumerable<TfsProjectMetrics>, IEnumerable>> GetReduce()
			{
				return Reduce;
			}
		}
	}
}