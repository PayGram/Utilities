using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.General
{
	public static class GeneralUtilities
	{
		public static DateTime FromUnixTime(int time)
		{
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(time);
			return dtDateTime;
		}
		/// <summary>
		/// Gets the next DateTime when a loop starting at startAtMinute and repeating every recurEveryMinutes will recurr
		/// </summary>
		/// <param name="startAtMinute">The first minute in the hour when the loop happens</param>
		/// <param name="recurEveryMinutes">How frequently (in minutes) the loop recurrs</param>
		/// <returns>The next time when the loop will recurr</returns>
		public static DateTime NextTime(this DateTime now, int startAtMinute, int recurEveryMinutes)
		{
			DateTime hourBegin = new DateTime(now.Year, now.Month, now.Day, now.Hour, startAtMinute, 0);
			var sinceHour = now - hourBegin;
			int millisSinceHour = (int)sinceHour.TotalMilliseconds;
			int intervalMillis = recurEveryMinutes * 60 * 1000;
			int lastRoundMinute = millisSinceHour == 0 ? 0 : (millisSinceHour / intervalMillis * recurEveryMinutes);
			DateTime nextRound = hourBegin.AddMinutes(lastRoundMinute + recurEveryMinutes);
			return nextRound;
		}
	}
}
