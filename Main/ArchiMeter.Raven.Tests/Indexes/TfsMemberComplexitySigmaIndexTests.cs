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

	public class TfsMemberComplexitySigmaIndexTests : IndexTestBase<TfsMetricsDocument, MemberComplexityDeviation, TfsMemberComplexitySigmaIndexTests.FakeTfsMemberComplexitySigmaIndexIndex>
	{
		[Test]
		public void WhenReducingThenCreatesGroupedData()
		{
			var data = new[]
				           {
					           new TfsMetricsDocument
						           {
							           ProjectName = "DocName",
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
												                     new TypeMetric(
													                     TypeMetricKind.Unknown,
													                     new[]
														                     {
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 10, 0, 10, "Member1", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 20, 0, 20, "Member2", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 30, 0, 30, "Member3", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 25, 0, 25, "Member4", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 15, 0, 15, "Member5", 0, new TypeCoupling[0], 0, 0)
														                     },
													                     10,
													                     0,
													                     0,
													                     0,
													                     new TypeCoupling[0],
													                     "Type1"),
												                     new TypeMetric(
													                     TypeMetricKind.Unknown,
													                     new[]
														                     {
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 10, 0, 10, "MemberA", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 20, 0, 20, "MemberB", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 30, 0, 30, "MemberC", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 25, 0, 25, "MemberD", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 15, 0, 15, "MemberE", 0, new TypeCoupling[0], 0, 0)
														                     },
													                     10,
													                     0,
													                     0,
													                     0,
													                     new TypeCoupling[0],
													                     "Type2")
											                     })
								                     }
						           }
				           };

			var result = PerformMapReduce(data);

			Assert.AreEqual(10, result.Length);
		}

		public class FakeTfsMemberComplexitySigmaIndexIndex : TfsMemberComplexitySigmaIndex, ITestIndex<TfsMetricsDocument, MemberComplexityDeviation>
		{
			public Expression<Func<IEnumerable<TfsMetricsDocument>, IEnumerable>> GetMap()
			{
				return Map;
			}

			public Expression<Func<IEnumerable<MemberComplexityDeviation>, IEnumerable>> GetReduce()
			{
				return Reduce;
			}
		}
	}
}