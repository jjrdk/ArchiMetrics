// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMetricConverter.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeCouplingConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Raven
{
	using System;
	using Common.Metrics;
	using global::Raven.Imports.Newtonsoft.Json;
	using global::Raven.Json.Linq;

	internal class TypeCouplingConverter:JsonConverterBase<TypeCoupling>
	{
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jObj = RavenJObject.Load(reader);
			return new TypeCoupling(
				jObj.Value<string>("ClassName"), 
				jObj.Value<string>("Namespace"), 
				jObj.Value<string>("Assembly"));
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(TypeCoupling).IsAssignableFrom(objectType);
		}
	}

	internal class TypeMetricConverter : JsonConverterBase<TypeMetric>
	{
		public override object ReadJson(JsonReader reader, 
										Type objectType, 
										object existingValue, 
										JsonSerializer serializer)
		{
			var jObj = RavenJObject.Load(reader);

			var couplings = serializer.Deserialize<TypeCoupling[]>(new RavenJTokenReader(jObj["ClassCouplings"]));
			var metrics = serializer.Deserialize<MemberMetric[]>(new RavenJTokenReader(jObj["MemberMetrics"]));
			return new TypeMetric(
				(TypeMetricKind)Enum.Parse(typeof(TypeMetricKind), jObj.Value<string>("Kind")), 
				metrics, 
				jObj.Value<int>("LinesOfCode"), 
				jObj.Value<int>("CyclomaticComplexity"), 
				jObj.Value<double>("MaintainabilityIndex"), 
				jObj.Value<int>("DepthOfInheritance"), 
				couplings, 
				jObj.Value<string>("Name"));
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(TypeMetric).IsAssignableFrom(objectType);
		}
	}
}
