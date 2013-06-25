// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberMetricConverter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberMetricConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Raven
{
	using System;
	using Common.Metrics;
	using global::Raven.Imports.Newtonsoft.Json;
	using global::Raven.Json.Linq;

	internal class MemberMetricConverter : JsonConverterBase<MemberMetric>
	{
		public override object ReadJson(JsonReader reader, 
										Type objectMember, 
										object existingValue, 
										JsonSerializer serializer)
		{
			var jObj = RavenJObject.Load(reader);

			var couplings = serializer.Deserialize<TypeCoupling[]>(new RavenJTokenReader(jObj["ClassCouplings"]));
			var metrics = serializer.Deserialize<IHalsteadMetrics>(new RavenJTokenReader(jObj["Halstead"]));
			return new MemberMetric(
				jObj.Value<string>("CodeFile"), 
				metrics, 
				(MemberMetricKind)Enum.Parse(typeof(MemberMetricKind), jObj.Value<string>("Kind")), 
				jObj.Value<int>("LineNumber"), 
				jObj.Value<int>("LinesOfCode"), 
				jObj.Value<double>("MaintainabilityIndex"), 
				jObj.Value<int>("CyclomaticComplexity"), 
				jObj.Value<string>("Name"), 
				jObj.Value<int>("LogicalComplexity"), 
				couplings, 
				jObj.Value<int>("NumberOfParameters"), 
				jObj.Value<int>("NumberOfLocalVariables"));
		}

		public override bool CanConvert(Type objectMember)
		{
			return typeof(MemberMetric).IsAssignableFrom(objectMember);
		}
	}
}
