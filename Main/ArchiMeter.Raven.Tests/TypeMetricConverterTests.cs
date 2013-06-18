// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMetricConverterTests.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeMetricConverterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven.Tests
{
	using System.IO;
	using System.Linq;
	using System.Text;
	using Common.Metrics;
	using global::Raven.Imports.Newtonsoft.Json;
	using NUnit.Framework;

	public class TypeMetricConverterTests
	{
		private TypeMetricConverter _converter;
		private JsonSerializer _jsonSerializer;

		[SetUp]
		public void Setup()
		{
			_converter = new TypeMetricConverter();
			_jsonSerializer = new JsonSerializer();
			_jsonSerializer.Converters.Add(_converter);
			_jsonSerializer.Converters.Add(new TypeCouplingConverter());
			_jsonSerializer.Converters.Add(new MemberMetricConverter());
		}

		[Test]
		public void CanConvertTypeMetrics()
		{
			Assert.IsTrue(_converter.CanConvert(typeof(TypeMetric)));
		}

		[Test]
		public void CanSerializeInstance()
		{
			const string Expected =
				"{\"Kind\":1,\"MemberMetrics\":[],\"LinesOfCode\":123,\"CyclomaticComplexity\":3,\"MaintainabilityIndex\":99.8,\"DepthOfInheritance\":2,\"ClassCouplings\":[{\"ClassName\":\"a\",\"Namespace\":\"b\",\"Assembly\":\"c\"}],\"ClassCoupling\":1,\"Name\":\"instance\"}";
			var output = new StringBuilder();
			var instance = new TypeMetric(TypeMetricKind.Class, new MemberMetric[0], 123, 3, 99.8, 2, new[] { new TypeCoupling("a", "b", "c") }, "instance");

			_converter.WriteJson(new JsonTextWriter(new StringWriter(output)), instance, _jsonSerializer);

			Assert.AreEqual(Expected, output.ToString());
		}

		[Test]
		public void CanDeserializeInstance()
		{
			const string Json =
				"{\"Kind\":1,\"MemberMetrics\":[],\"LinesOfCode\":123,\"CyclomaticComplexity\":3,\"MaintainabilityIndex\":99.8,\"DepthOfInheritance\":2,\"ClassCouplings\":[{\"ClassName\":\"a\",\"Namespace\":\"b\",\"Assembly\":\"c\"}],\"ClassCoupling\":1,\"Name\":\"instance\"}";
			var reader = new JsonTextReader(new StringReader(Json));
			var result = (TypeMetric)_converter.ReadJson(reader, typeof(TypeMetric), null, _jsonSerializer);

			Assert.AreEqual(99.8, result.MaintainabilityIndex);
		}

		[Test]
		public void WhenDeserializingInstanceThenClassCouplingsHaveNamespace()
		{
			const string Json =
				"{\"Kind\":1,\"MemberMetrics\":[],\"LinesOfCode\":123,\"CyclomaticComplexity\":3,\"MaintainabilityIndex\":99.8,\"DepthOfInheritance\":2,\"ClassCouplings\":[{\"ClassName\":\"a\",\"Namespace\":\"b\",\"Assembly\":\"c\"}],\"ClassCoupling\":1,\"Name\":\"instance\"}";
			var reader = new JsonTextReader(new StringReader(Json));
			var result = (TypeMetric)_converter.ReadJson(reader, typeof(TypeMetric), null, _jsonSerializer);

			Assert.False(result.ClassCouplings.Any(c => string.IsNullOrWhiteSpace(c.Namespace)));
		}
	}
}