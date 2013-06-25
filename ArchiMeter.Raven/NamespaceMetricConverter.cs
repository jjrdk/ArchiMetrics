// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceMetricConverter.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceMetricConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Raven
{
	using System;
	using Common.Metrics;
	using global::Raven.Imports.Newtonsoft.Json;
	using global::Raven.Json.Linq;

	internal class NamespaceMetricConverter : JsonConverterBase<NamespaceMetric>
	{
		public override object ReadJson(JsonReader reader, 
		                                Type objectType, 
		                                object existingValue, 
		                                JsonSerializer serializer)
		{
			var jObj = RavenJObject.Load(reader);

			var couplings = serializer.Deserialize<TypeCoupling[]>(new RavenJTokenReader(jObj["ClassCouplings"]));
			var metrics = serializer.Deserialize<TypeMetric[]>(new RavenJTokenReader(jObj["TypeMetrics"]));
			return new NamespaceMetric(
				jObj.Value<double>("MaintainabilityIndex"), 
				jObj.Value<int>("CyclomaticComplexity"), 
				jObj.Value<int>("LinesOfCode"), 
				couplings, 
				jObj.Value<int>("DepthOfInheritance"), 
				jObj.Value<string>("Name"), 
				metrics);
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(NamespaceMetric).IsAssignableFrom(objectType);
		}
	}

	// internal class EvaluationResultDocumentConverter : JsonConverterBase<NamespaceMetric>
	// {
	// 	public override object ReadJson(JsonReader reader,
	// 									Type objectType,
	// 									object existingValue,
	// 									JsonSerializer serializer)
	// 	{
	// 		var jObj = RavenJObject.Load(reader);

	// 		var couplings = serializer.Deserialize<TypeCoupling[]>(new RavenJTokenReader(jObj["ClassCouplings"]));
	// 		var metrics = serializer.Deserialize<TypeMetric[]>(new RavenJTokenReader(jObj["TypeMetrics"]));
	// 		return new NamespaceMetric(
	// 			jObj.Value<double>("MaintainabilityIndex"),
	// 			jObj.Value<int>("CyclomaticComplexity"),
	// 			jObj.Value<int>("LinesOfCode"),
	// 			couplings,
	// 			jObj.Value<int>("DepthOfInheritance"),
	// 			jObj.Value<string>("Name"),
	// 			metrics);
	// 	}

	// 	public override bool CanConvert(Type objectType)
	// 	{
	// 		return typeof(NamespaceMetric).IsAssignableFrom(objectType);
	// 	}
	// }
}