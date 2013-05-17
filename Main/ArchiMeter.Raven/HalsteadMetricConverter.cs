// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HalsteadMetricConverter.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the HalsteadMetricConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Raven
{
	using System;
	using CodeReview.Metrics;
	using Common.Metrics;
	using global::Raven.Imports.Newtonsoft.Json;
	using global::Raven.Json.Linq;

	internal class HalsteadMetricConverter : JsonConverterBase<HalsteadMetrics>
	{
		public override object ReadJson(JsonReader reader, 
		                                Type objectType, 
		                                object existingValue, 
		                                JsonSerializer serializer)
		{
			var jObj = RavenJObject.Load(reader);

			return new HalsteadMetrics(
				jObj.Value<int>("NumberOfOperands"), 
				jObj.Value<int>("NumberOfOperators"), 
				jObj.Value<int>("NumberOfUniqueOperands"), 
				jObj.Value<int>("NumberOfUniqueOperators"));
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(IHalsteadMetrics).IsAssignableFrom(objectType);
		}
	}
}