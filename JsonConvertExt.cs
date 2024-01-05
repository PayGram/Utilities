using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace Utilities.String.Json.Extentions
{
	public static class JsonConvertExt
	{
		public static string? SerializeIgnoreAndPopulate<T>(this T obj)
		{
			if (obj == null) return null;
			var settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate };
			return JsonConvert.SerializeObject(obj, Formatting.None, settings);
		}

		public static string? Stringify<T>(this T obj)
		{
			if (obj == null) return null;
			StringBuilder sb = new StringBuilder();
			foreach (var prop in obj.GetType().GetProperties(/*BindingFlags.Public*/))
			{
				string? value;
				if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string))
					value = prop.GetValue(obj)?.ToString();
				else
					value = "\r\n" + prop.GetValue(obj).Stringify();

				sb.Append(prop.Name);
				sb.Append(": ");
				sb.AppendLine(value);
			}
			return sb.ToString();
		}
		public static T? DeserializeObject<T>(this string value)
		{
			if (string.IsNullOrWhiteSpace(value)) return default;
			try
			{
				return JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate });
			}
			catch { return default; }
		}

		public static T? DeserializeObject<T>(this string value, JsonSerializerSettings settings)
		{
			if (string.IsNullOrWhiteSpace(value)) return default;
			try
			{
				return JsonConvert.DeserializeObject<T>(value, settings);
			}
			catch { return default; }
		}

		public static T? DeserializeObject<T>(this string value, JsonConverter[] converters)
		{
			if (string.IsNullOrWhiteSpace(value)) return default;
			try
			{
				return JsonConvert.DeserializeObject<T>(value, converters);
			}
			catch { return default; }
		}

	}
}