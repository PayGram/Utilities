using System;
using System.Data;
using System.Security.Cryptography;
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
			if (string.IsNullOrWhiteSpace(base64EncodedData)) return null;
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
		/// <summary>
		/// Returns a random string of the desired lenght.
		/// if addNumbers =false and addChars=false, null is returned
		/// </summary>
		/// <param name="len">must be >=0 otherwise null is returned</param>
		/// <param name="addNumbers"></param>
		/// <param name="addChars"></param>
		/// <param name="caseSensitive"></param>
		/// <returns>A random string of the requested length</returns>
		/// <exception cref="InvalidOperationException">When addNumbers and addChars are false</exception>
		public static string GetRandomString(int len
			, bool addNumbers = true
			, bool addChars = true
			, bool caseSensitive = false
			, bool removeZeroAndO = false)
		{
			const string numbersSource = "123456789";
			const string digitsSource = "abcdefghjkilmnpqrstuvwxyz";
			const string zero = "0";
			const string o = "o";

			if (addNumbers == false && addChars == false)
				throw new InvalidOperationException("addChars or addNumbers must be true");

			var source = new StringBuilder();
			if (addChars)
			{
				source.Append( digitsSource);
				if (removeZeroAndO == false) source.Append(o);
			}

			if (caseSensitive)
				source.Append(source.ToString().ToUpper());

			if (addNumbers)
			{
				source.Append(numbersSource);
				if (removeZeroAndO == false) source.Append(zero);
			}

			if (len < 0) return "";
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < len; i++)
			{
				sb.Append(source[RandomNumberGenerator.GetInt32(source.Length)]);
			}
			return sb.ToString();
		}
	}
}
