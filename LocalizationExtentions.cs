using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using Utilities.String.Extentions;

namespace Utilities.Localization.Extentions
{
    public static class LocalizationExtentions
    {
        static ResourceManager? mng = null;
        readonly static Dictionary<string, CultureInfo> cultures = new Dictionary<string, CultureInfo>();
        readonly static object sync = new object();
        /// <summary>
        /// Gets or sets the resource manager used by Translate to localize
        /// </summary>
        public static ResourceManager? ResourceManager { get => mng; set => mng = value; }
        public static string? Translate(this ResourceManager rscmng, FormattableString str, CultureInfo culture)
        {
            if (rscmng == null || culture == null || str == null || string.IsNullOrWhiteSpace(str.ToString())) return str?.ToString();
            var transl = rscmng.GetString(str.Format, culture);
            if (string.IsNullOrWhiteSpace(transl)) return str.ToString();
            object[] args = (object[])(str.GetArguments() ?? Array.Empty<object>());
            return transl.Format(culture, args);
        }
        public static string? Translate(this FormattableString str, CultureInfo culture)
        {
            if (culture == null || str == null || string.IsNullOrEmpty(str.ToString())) return str?.ToString();
            if (mng == null)
                throw new InvalidOperationException("Set LocalizationExtentions.ResourceManager before calling this method");
            return Translate(mng, str, culture);
        }
        public static string? TranslateString(this ResourceManager? rscmng, CultureInfo? culture, string? str, params object?[] args)
        {
            if (args == null) args = Array.Empty<object>();
            if (rscmng == null || culture == null || str == null || string.IsNullOrWhiteSpace(str))
                return culture == null ? str : str?.Format(culture, args!);
            var transl = rscmng.GetString(str, culture);
            if (string.IsNullOrWhiteSpace(transl)) transl = str;
            return transl.Format(culture, args!);// string.Format(rscmng.GetString(str, culture), arguments) ?? str;
        }
        public static string? Translate(this string str, CultureInfo culture, params object[] arguments)
        {
            return TranslateString(mng, culture, str, arguments);
            //if (culture == null || str == null || string.IsNullOrEmpty(str)) return str;
            //if (mng == null)
            //	throw new InvalidOperationException("Set LocalizationExtentions.ResourceManager before calling this method");
            //return TranslateString(mng, culture, str, arguments);
        }
        public static string? TranslateString(this CultureInfo culture, string str, params object[] arguments)
        {
            return TranslateString(mng, culture, str, arguments);
            //if (str == null || string.IsNullOrWhiteSpace(str)) return str;
            //if (culture == null) return str.Format(arguments);
            //if (mng == null)
            //	throw new InvalidOperationException("Set LocalizationExtentions.ResourceManager before calling this method");
            //return string.Format(mng.GetString(str, culture), arguments) ?? str;
        }
        public static string? Translate(this CultureInfo culture, FormattableString str)
        {
            if (culture == null || str == null || string.IsNullOrEmpty(str.ToString())) return str.ToString();
            if (mng == null)
                throw new InvalidOperationException("Set LocalizationExtentions.ResourceManager before calling this method");
            return Translate(mng, str, culture);
        }
        public static string? Translate(this ResourceManager rscmng, FormattableString str, string culture)
        {
            if (rscmng == null || string.IsNullOrEmpty(culture) || str == null || string.IsNullOrWhiteSpace(str.ToString())) return str?.ToString();
            return TranslateString(rscmng, GetOrAddCulture(culture), str.Format, str.GetArguments());
        }
        public static string? Translate(this FormattableString str, string culture)
        {
            if (string.IsNullOrEmpty(culture) || str == null || string.IsNullOrEmpty(str.ToString())) return str?.ToString();
            if (mng == null)
                throw new InvalidOperationException("Set LocalizationExtentions.ResourceManager before calling this method");
            return Translate(mng, str, culture);
        }
        public static string? TranslateString(this ResourceManager rscmng, string culture, string str, params object[] arguments)
        {
            if (string.IsNullOrEmpty(str)) return str;
            if (string.IsNullOrEmpty(culture) || rscmng == null) return str.Format(arguments) ?? str;
            return TranslateString(mng, GetOrAddCulture(culture), str, arguments);
        }
        public static string? Translate(this string str, string culture, params object[] arguments)
        {
            if (string.IsNullOrEmpty(str)) return str;
            if (string.IsNullOrEmpty(culture)) return str.Format(arguments) ?? str;
            if (mng == null)
                throw new InvalidOperationException("Set LocalizationExtentions.ResourceManager before calling this method");
            return TranslateString(mng, GetOrAddCulture(culture), str, arguments);
        }
        static CultureInfo? GetOrAddCulture(string culture)
        {
            if (string.IsNullOrWhiteSpace(culture)) return null;
            lock (sync)
            {
                if (cultures.ContainsKey(culture)) return cultures[culture];
                var ci = new CultureInfo(culture);
                cultures.Add(culture, ci);
                return ci;
            }
        }
    }
}
