// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConverterBase.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the JsonConverterBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Raven
{
	using global::Raven.Imports.Newtonsoft.Json;

	internal abstract class JsonConverterBase<T> : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, 
									   object value, 
									   JsonSerializer serializer)
		{
			var metric = (T)value;
			writer.WriteStartObject();
			var properties = typeof(T).GetProperties();
			foreach (var property in properties)
			{
				writer.WritePropertyName(property.Name);
				serializer.Serialize(writer, property.GetValue(metric));

				// var json = JsonConvert.SerializeObject(property.GetValue(metric), serializer.Converters.ToArray());
				// writer.WriteRawValue(json);
			}

			writer.WriteEndObject();
		}
	}
}