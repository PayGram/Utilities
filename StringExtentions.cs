using System;
using System.Text;

namespace Utilities.String.Extentions
{
	public static class StringExtentions
	{
		#region enconding
		public static string Base64Encode(this string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}
		public static string Base64Decode(this string base64EncodedData)
		{
			try
			{
				var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
				return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
			}
			catch (Exception)
			{
				return null;
			}
		}
		public static string Base85Encode(this string str)
		{
			return Ascii85.Encode(Encoding.ASCII.GetBytes(str));
		}
		public static string Base85Decode(this string encoded)
		{
			try
			{
				return Encoding.ASCII.GetString(Ascii85.Decode(encoded));
			}
			catch (Exception) { return null; }
		}
		#endregion
		/// <summary>
		/// Formats the string and doesn't throw if arguments are not of the right number, instead it returns null in this case.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static string Format(this string format, params object[] args)
		{
			if (string.IsNullOrWhiteSpace(format) || args == null || args.Length == 0)
				return format;
			try
			{
				return string.Format(format, args);
			}
			catch
			{
				return null;
			}
		}
		public static string Format(this string format, IFormatProvider culture, params object[] args)
		{
			if (string.IsNullOrWhiteSpace(format) || args == null)
				return format;
			if (culture == null)
				return format.Format(args);
			try
			{
				return string.Format(culture, format, args);
			}
			catch
			{
				return null;
			}
		}
	}
}
