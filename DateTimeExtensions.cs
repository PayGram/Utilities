namespace Utilities.String.Extentions
{
	public static class DateTimeExtensions
	{
		public static DateTime UtcToTimeZone(this DateTime utcDateTime, string timeZoneId)
		{
			var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
		}
	}
}
