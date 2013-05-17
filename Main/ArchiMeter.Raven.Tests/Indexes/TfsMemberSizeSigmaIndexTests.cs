namespace ArchiMeter.Raven.Tests.Indexes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	using ArchiMeter.Common.Documents;

	using Common.Metrics;

	using NUnit.Framework;
	using Raven.Indexes;
	using global::Raven.Imports.Newtonsoft.Json;

	public class TfsMemberSizeSigmaIndexTests
	{
		private FakeTfsMemberSizeSigmaIndex _index;

		[SetUp]
		public void Setup()
		{
			_index = new FakeTfsMemberSizeSigmaIndex();
		}

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
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 10, 0, 0, "Member1", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 20, 0, 0, "Member2", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 30, 0, 0, "Member3", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 25, 0, 0, "Member4", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 15, 0, 0, "Member5", 0, new TypeCoupling[0], 0, 0)
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
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 10, 0, 0, "MemberA", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 20, 0, 0, "MemberB", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 30, 0, 0, "MemberC", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 25, 0, 0, "MemberD", 0, new TypeCoupling[0], 0, 0),
															                     new MemberMetric("", null, MemberMetricKind.Unknown, 0, 15, 0, 0, "MemberE", 0, new TypeCoupling[0], 0, 0)
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

		private object[] PerformMapReduce(IEnumerable<TfsMetricsDocument> docs)
		{
			var mapped = _index.GetMap().Compile()(docs);
			var json = JsonConvert.SerializeObject(mapped);
			var errors = JsonConvert.DeserializeObject<MemberSizeDeviation[]>(json);
			var reduced = _index.GetReduce().Compile()(errors);
			var results = reduced.OfType<object>().ToArray();
			return results;
		}

		public class FakeTfsMemberSizeSigmaIndex : TfsMemberSizeSigmaIndex, ITestIndex<TfsMetricsDocument, MemberSizeDeviation>
		{
			public Expression<Func<IEnumerable<TfsMetricsDocument>, IEnumerable>> GetMap()
			{
				return Map;
			}

			public Expression<Func<IEnumerable<MemberSizeDeviation>, IEnumerable>> GetReduce()
			{
				return Reduce;
			}
		}
	}
}