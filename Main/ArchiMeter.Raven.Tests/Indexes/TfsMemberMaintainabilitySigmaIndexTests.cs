namespace ArchiMeter.Raven.Tests.Indexes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	using ArchiMeter.Common.Documents;
	using ArchiMeter.Common.Metrics;
	using ArchiMeter.Raven.Indexes;

	using NUnit.Framework;

	using global::Raven.Imports.Newtonsoft.Json;

	public class TfsMemberMaintainabilitySigmaIndexTests : IndexTestBase<TfsMetricsDocument, MemberMaintainabilityDeviation, TfsMemberMaintainabilitySigmaIndexTests.FakeTfsMemberMaintainabilitySigmaIndexIndex>
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
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 10, 10, 10, "Member1", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 20, 20, 20, "Member2", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 30, 30, 30, "Member3", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 25, 25, 25, "Member4", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 15, 15, 15, "Member5", 0, new TypeCoupling[0], 0, 0)
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
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 10, 10, 10, "MemberA", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 20, 20, 20, "MemberB", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 30, 30, 30, "MemberC", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 25, 25, 25, "MemberD", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 15, 15, 15, "MemberE", 0, new TypeCoupling[0], 0, 0)
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

		public class FakeTfsMemberMaintainabilitySigmaIndexIndex : TfsMemberMaintainabilitySigmaIndex, ITestIndex<TfsMetricsDocument, MemberMaintainabilityDeviation>
		{
			public Expression<Func<IEnumerable<TfsMetricsDocument>, IEnumerable>> GetMap()
			{
				return Map;
			}

			public Expression<Func<IEnumerable<MemberMaintainabilityDeviation>, IEnumerable>> GetReduce()
			{
				return Reduce;
			}
		}
	}
}