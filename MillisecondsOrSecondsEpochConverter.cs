using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Utilities.String.Json
{
	public class MillisecondsOrSecondsEpochConverter : DateTimeConverterBase
	{
		private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteRawValue(((DateTime)value - _epoch).TotalMilliseconds.ToString("0"));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null) { return null; }
			if (reader.Value.ToString().Length > 10)
				return _epoch.AddMilliseconds((long)reader.Value);
			else
				return _epoch.AddSeconds((long)reader.Value);
		}
	}
}
