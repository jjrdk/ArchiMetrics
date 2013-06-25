// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceMetricConverterTests.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceMetricConverterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Raven.Tests
{
	using System.IO;
	using System.Text;
	using Common.Metrics;
	using global::Raven.Imports.Newtonsoft.Json;
	using NUnit.Framework;

	public class NamespaceMetricConverterTests
	{
		private NamespaceMetricConverter _converter;
		private JsonSerializer _jsonSerializer;

		[SetUp]
		public void Setup()
		{
			_converter = new NamespaceMetricConverter();
			_jsonSerializer = new JsonSerializer();
			_jsonSerializer.Converters.Add(_converter);
			_jsonSerializer.Converters.Add(new TypeMetricConverter());
			_jsonSerializer.Converters.Add(new MemberMetricConverter());
		}

		[Test]
		public void CanConvertNamespaceMetrics()
		{
			Assert.IsTrue(_converter.CanConvert(typeof(NamespaceMetric)));
		}

		[Test]
		public void CanSerializeInstance()
		{
			const string Expected =
				"{\"MaintainabilityIndex\":99.8,\"CyclomaticComplexity\":3,\"LinesOfCode\":123,\"ClassCouplings\":[{\"ClassName\":\"a\",\"Namespace\":\"b\",\"Assembly\":\"c\"}],\"DepthOfInheritance\":2,\"Name\":\"instance\",\"TypeMetrics\":[]}";
			var output = new StringBuilder();
			var instance = new NamespaceMetric(99.8, 3, 123, new[] { new TypeCoupling("a", "b", "c") }, 2, "instance", new TypeMetric[0]);

			_converter.WriteJson(new JsonTextWriter(new StringWriter(output)), instance, _jsonSerializer);

			Assert.AreEqual(Expected, output.ToString());
		}

		[Test]
		public void CanDeserializeInstance()
		{
			const string Json =
				"{\"MaintainabilityIndex\":99.8,\"CyclomaticComplexity\":3,\"LinesOfCode\":123,\"ClassCouplings\":[{\"ClassName\":\"a\",\"Namespace\":\"b\",\"Assembly\":\"c\"}],\"DepthOfInheritance\":2,\"Name\":\"instance\",\"TypeMetrics\":[]}";
			var reader = new JsonTextReader(new StringReader(Json));
			var result = (NamespaceMetric)_converter.ReadJson(reader, typeof(NamespaceMetric), null, _jsonSerializer);

			Assert.AreEqual(99.8, result.MaintainabilityIndex);
		}
	}
}
