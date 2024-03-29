﻿namespace Utilities.General
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
		/// Gets the next DateTime when a loop starting at startAtMinute of every hour and repeating every recurEveryMinutes will recur
		/// </summary>
		/// <param name="startAtMinute">The first minute in the hour when the loop happens</param>
		/// <param name="recurEveryMinutes">How frequently (in minutes) the loop recurs</param>
		/// <returns>The next time when the loop will recur</returns>
		public static DateTime NextTime(this DateTime now, int startAtMinute, int recurEveryMinutes)
		{
			if (startAtMinute < 0 || startAtMinute > 59) throw new ArgumentException("startAtMinute must be >= 0 and <= 59");
			if (recurEveryMinutes <= 0 || recurEveryMinutes > 60) throw new ArgumentException("recurEveryMinutes must be >0 and <=60");
			if (60 % recurEveryMinutes != 0) throw new ArgumentException("60 must be divisible by recuverEveryMinutes");

			//DateTime firstTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, startAtMinute, 0, now.Kind);
			//if (firstTime > now)
			//	firstTime = firstTime.AddHours(-1);
			////DateTime prevTime = firstTme;
			//DateTime nextTime = firstTime.AddMinutes(recurEveryMinutes);
			//while (nextTime <= now)
			//{
			//	nextTime = nextTime.AddMinutes(recurEveryMinutes);
			//}
			//return nextTime;

			//same as the above code but no loop
			DateTime hourBegin = new DateTime(now.Year, now.Month, now.Day, now.Hour, startAtMinute, 0);
			if (hourBegin > now)
				hourBegin = hourBegin.AddHours(-1);
			var sinceHour = now - hourBegin;
			int millisSinceHour = (int)sinceHour.TotalMilliseconds;
			int intervalMillis = recurEveryMinutes * 60 * 1000;
			int lastRoundMinute = millisSinceHour == 0 ? 0 : (millisSinceHour / intervalMillis * recurEveryMinutes);
			DateTime nextRound = hourBegin.AddMinutes(lastRoundMinute + recurEveryMinutes);
			return nextRound;
		}

		/// <summary>
		/// Gets the closes number to num, divisible by divBy. 
		/// </summary>
		/// <param name="num">The number to be rounded</param>
		/// <param name="divBy">The divisor</param>
		/// <param name="decimals">How many decimals should be there on the final result</param>
		/// <param name="rounding">The rounding method</param>
		/// <returns>A number which is divisible by divBy or zero. if decimals is less than 0, num is returned</returns>
		public static decimal ClosestDivisible(this decimal num, int divBy, int decimals, MidpointRounding rounding)
		{
			if (divBy == 0) return num;
			if (decimals < 0) decimals = 0;

			// rounds the number
			var rounded = Math.Round(num, decimals, rounding);

			// we will make the number without decimals be divisible by divBy
			long noDecimals = (long)(rounded * (decimal)Math.Pow(10, decimals));

			long smallestDivisible = (int)(noDecimals / divBy) * divBy;
			long rem = noDecimals % divBy;
			bool remBiggerThanMid = Math.Abs(rem) >= (double)divBy / 2;

			if (rem != 0)
				switch (rounding)
				{
					case MidpointRounding.AwayFromZero:
						if (smallestDivisible >= 0)
						{
							if (remBiggerThanMid)
								smallestDivisible += divBy;
							// else ok
						}
						else
						{
							if (remBiggerThanMid)
								smallestDivisible -= divBy;
							//else ok
						}
						break;
					//case MidpointRounding.ToZero:
					//	//if(smallestDivisible>0) nothing to do, it is already to zero
					//	if (smallestDivisible < 0)
					//		smallestDivisible += divBy;
					//	break;
					//case MidpointRounding.ToNegativeInfinity:
					//	smallestDivisible = smallestDivisible - divBy;
					//	break;
					//case MidpointRounding.ToPositiveInfinity:
					//	smallestDivisible = smallestDivisible + divBy;
					//	break;
					case MidpointRounding.ToEven:
						if (smallestDivisible >= 0)
						{
							if (remBiggerThanMid)
							{
								if (remBiggerThanMid && (smallestDivisible + divBy) % 2 == 0)
									smallestDivisible += divBy;
								//else ok, smallestDivisible is even
							}
						}
						else
						{
							if (remBiggerThanMid && (smallestDivisible - divBy) % 2 == 0)
								smallestDivisible -= divBy;
						}
						break;
				}
			decimal toret = smallestDivisible / (decimal)Math.Pow(10, decimals);
			return toret;
		}
		/// <summary>
		/// Gets  the closest divisible number which is greater than or equal to a specified value
		/// </summary>
		/// <param name="num"></param>
		/// <param name="divBy"></param>
		/// <param name="decimals"></param>
		/// <param name="rounding"></param>
		/// <param name="gtEqTo">the minimum number that can be returned</param>
		/// <returns></returns>
		public static decimal ClosestDivisibleMin(this decimal num, int divBy, int decimals, decimal gtEqTo)
		{
			if (num < gtEqTo)
			{
				num = gtEqTo;
			}
			var res = ClosestDivisible(num, divBy, decimals, MidpointRounding.AwayFromZero);
			if (res < gtEqTo)
			{
				var add = divBy / Math.Pow(10, decimals);
				res += (decimal)add;
			}
			return res;
		}/// <summary>
		 /// Gets  the closest divisible number which is greater than or equal to a specified value
		 /// </summary>
		 /// <param name="num"></param>
		 /// <param name="divBy"></param>
		 /// <param name="decimals"></param>
		 /// <param name="rounding"></param>
		 /// <param name="ltEqTo">the maximum number that can be returned</param>
		 /// <returns></returns>
		public static decimal ClosestDivisibleMax(this decimal num, int divBy, int decimals, decimal ltEqTo)
		{
			if (num > ltEqTo)
			{
				num = ltEqTo;
			}
			var res = ClosestDivisible(num, divBy, decimals, MidpointRounding.AwayFromZero);
			if (res > ltEqTo)
			{
				var add = divBy / Math.Pow(10, decimals);
				res -= (decimal)add;
			}
			return res;
		}
		public static bool IsValidEmail(string email)
		{
			if (string.IsNullOrWhiteSpace(email)) return false;
			var trimmedEmail = email.Trim();

			if (trimmedEmail.EndsWith("."))
			{
				return false; // suggested by @TK-421
			}
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == trimmedEmail;
			}
			catch
			{
				return false;
			}
		}
	}
}
