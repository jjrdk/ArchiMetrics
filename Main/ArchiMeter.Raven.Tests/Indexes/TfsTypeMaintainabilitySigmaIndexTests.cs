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

	public class TfsTypeMaintainabilitySigmaIndexTests : IndexTestBase<TfsMetricsDocument, TypeMaintainabilityDeviation, TfsTypeMaintainabilitySigmaIndexTests.FakeTfsTypeMaintainabilitySigmaIndex>
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
												                     new TypeMetric(
													                     TypeMetricKind.Unknown,
													                     new MemberMetric[0],
													                     10,
													                     10,
													                     10,
													                     0,
													                     new TypeCoupling[0],
													                     "Type1"),
												                     new TypeMetric(
													                     TypeMetricKind.Unknown,
													                     new MemberMetric[0],
													                     20,
													                     20,
													                     20,
													                     0,
													                     new TypeCoupling[0],
													                     "Type2"),
												                     new TypeMetric(
													                     TypeMetricKind.Unknown,
													                     new MemberMetric[0],
													                     30,
													                     30,
													                     30,
													                     0,
													                     new TypeCoupling[0],
													                     "Type3"), 
												                     new TypeMetric(
													                     TypeMetricKind.Unknown,
													                     new MemberMetric[0],
													                     15,
													                     15,
													                     15,
													                     0,
													                     new TypeCoupling[0],
													                     "Type4"),
												                     new TypeMetric(
													                     TypeMetricKind.Unknown,
													                     new MemberMetric[0],
													                     25,
													                     25,
													                     25,
													                     0,
													                     new TypeCoupling[0],
													                     "Type5") 
											                     }) 
								                     }
						           }
				           };

			var result = PerformMapReduce(data);

			Assert.AreEqual(5, result.Length);
		}

		public class FakeTfsTypeMaintainabilitySigmaIndex : TfsTypeMaintainabilitySigmaIndex, ITestIndex<TfsMetricsDocument, TypeMaintainabilityDeviation>
		{
			public Expression<Func<IEnumerable<TfsMetricsDocument>, IEnumerable>> GetMap()
			{
				return Map;
			}

			public Expression<Func<IEnumerable<TypeMaintainabilityDeviation>, IEnumerable>> GetReduce()
			{
				return Reduce;
			}
		}
	}
}