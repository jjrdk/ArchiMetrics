// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCouplingConverterTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeCouplingConverterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Raven.Tests
{
	using System.IO;
	using System.Text;
	using Common.Metrics;
	using global::Raven.Imports.Newtonsoft.Json;
	using NUnit.Framework;

	public class TypeCouplingConverterTests
	{
		private TypeCouplingConverter _converter;
		private JsonSerializer _jsonSerializer;

		[SetUp]
		public void Setup()
		{
			_converter = new TypeCouplingConverter();
			_jsonSerializer = new JsonSerializer();
			_jsonSerializer.Converters.Add(_converter);
		}

		[Test]
		public void CanConvertTypeCouplings()
		{
			Assert.IsTrue(_converter.CanConvert(typeof(TypeCoupling)));
		}

		[Test]
		public void CanSerializeInstance()
		{
			const string Expected =
				"{\"ClassName\":\"SomeClass\",\"Namespace\":\"SomeNamespace\",\"Assembly\":\"SomeAssembly\"}";
			var output = new StringBuilder();
			var instance = new TypeCoupling("SomeClass", "SomeNamespace", "SomeAssembly");

			_converter.WriteJson(new JsonTextWriter(new StringWriter(output)), instance, _jsonSerializer);

			Assert.AreEqual(Expected, output.ToString());
		}

		[Test]
		public void CanDeserializeInstance()
		{
			const string Json =
				"{\"ClassName\":\"SomeClass\",\"Namespace\":\"SomeNamespace\",\"Assembly\":\"SomeAssembly\"}";
			var reader = new JsonTextReader(new StringReader(Json));
			var result = (TypeCoupling)_converter.ReadJson(reader, typeof(TypeCoupling), null, _jsonSerializer);

			Assert.AreEqual("SomeNamespace.SomeClass, SomeAssembly", result.ToString());
		}
	}
}
