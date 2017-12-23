using System;

namespace Coralcode.Framework.Extensions
{
    public static class DateTimeExtensions
    {
        #region 获取一些时间的问题
        public static DateTime ResetTimeToStart(this DateTime dateTime)
        {
            return dateTime.AddHours(-dateTime.Hour).AddMinutes(-dateTime.Millisecond).AddSeconds(-dateTime.Second);
        }
        public static DateTime ResetTimeToEnd(this DateTime dateTime)
        {
            return dateTime.AddDays(1).AddHours(-dateTime.Hour).AddMinutes(-dateTime.Millisecond).AddSeconds(-dateTime.Second);
        }

        public static DateTime GetDayStartTime(this DateTime dateTime)
        {

            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public static DateTime GetDayEndTime(this DateTime dateTime)
        {
            return dateTime.GetMonthStartTime().AddDays(1);
        }

        public static int GetWeek(this DateTime dateTime)
        {
            return new System.Globalization.GregorianCalendar().GetWeekOfYear(dateTime, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        public static DateTime GetWeekStartTime(this DateTime dateTime)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Sunday)
                return dateTime.AddDays(-6).ResetTimeToStart();
            return dateTime.AddDays(-(int)dateTime.DayOfWeek + 1).ResetTimeToStart();
        }

        public static DateTime GetWeekEndTime(this DateTime dateTime)
        {
            return dateTime.GetWeekStartTime().AddDays(7);
        }

        public static DateTime GetMonthStartTime(this DateTime dateTime)
        {

            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static DateTime GetMonthEndTime(this DateTime dateTime)
        {
            return dateTime.GetMonthStartTime().AddMonths(1);
        }

        public static int GetSeason(this DateTime dateTime)
        {
            if (dateTime.Month < 3)
                return 1;
            if (dateTime.Month > 3 && dateTime.Month < 7)
                return 2;
            if (dateTime.Month > 6 && dateTime.Month < 10)
                return 3;
            return 4;

        }

        public static DateTime GetSeasonStartTime(this DateTime dateTime)
        {
            if (dateTime.Month < 3)
                return new DateTime(dateTime.Year, 1, 1);
            if (dateTime.Month > 3 && dateTime.Month < 7)
                return new DateTime(dateTime.Year, 4, 1);
            if (dateTime.Month > 6 && dateTime.Month < 10)
                return new DateTime(dateTime.Year, 7, 1);
            return new DateTime(dateTime.Year, 10, 1);
        }

        public static DateTime GetSeasonEndTime(this DateTime dateTime)
        {
            if (dateTime.Month < 3)
                return new DateTime(dateTime.Year, 3, 31);
            if (dateTime.Month > 3 && dateTime.Month < 7)
                return new DateTime(dateTime.Year, 6, 30);
            if (dateTime.Month > 6 && dateTime.Month < 10)
                return new DateTime(dateTime.Year, 9, 30);
            return new DateTime(dateTime.Year, 12, 31);
        }

        public static DateTime GetYearStartTime(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1);
        }

        public static DateTime GetYearEndTime(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1).AddYears(1);
        }
        #endregion

        #region 获取Utc时间
        private static readonly DateTime MinDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long ToUnixTime(this DateTime dateTime)
        {
            return (long)(dateTime.ToStableUniversalTime() - MinDateTimeUtc).TotalSeconds;
        }

        public static DateTime FromUnixTime(this double unixTime)
        {
            return MinDateTimeUtc + TimeSpan.FromSeconds(unixTime);
        }

        public static DateTime ToStableUniversalTime(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;
            if (dateTime == DateTime.MinValue)
                return MinDateTimeUtc;
            return TimeZoneInfo.ConvertTimeToUtc(dateTime);
        }
        #endregion


    }
}
