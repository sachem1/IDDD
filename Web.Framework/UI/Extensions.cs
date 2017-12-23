using Iddd.Data.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Web.Framework
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
      

        /// <summary>
        /// Returns a value indicating whether real selection is not possible
        /// </summary>
        /// <param name="items">Items</param>
        /// <param name="ignoreZeroValue">A value indicating whether we should ignore items with "0" value</param>
        /// <returns>A value indicating whether real selection is not possible</returns>
        public static bool SelectionIsNotPossible(this IList<SelectListItem> items, bool ignoreZeroValue = true)
        {
            if (items == null)
                throw  new ArgumentNullException("items");

            //we ignore items with "0" value? Usually it's something like "Select All", "etc
            return items.Count(x => !ignoreZeroValue || !x.Value.ToString().Equals("0")) < 2;
        }

        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source)
        {
            return RelativeFormat(source, string.Empty);
        }
        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <param name="defaultFormat">Default format string (in case relative formatting is not applied)</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source, string defaultFormat)
        {
            return RelativeFormat(source, false, defaultFormat);
        }
        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <param name="convertToUserTime">A value indicating whether we should convet DateTime instance to user local time (in case relative formatting is not applied)</param>
        /// <param name="defaultFormat">Default format string (in case relative formatting is not applied)</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source,
            bool convertToUserTime, string defaultFormat)
        {
            string result = "";

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - source.Ticks);
            double delta = ts.TotalSeconds;

            if (delta > 0)
            {
                if (delta < 60) // 60 (seconds)
                {
                    result = ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
                }
                else if (delta < 120) //2 (minutes) * 60 (seconds)
                {
                    result = "a minute ago";
                }
                else if (delta < 2700) // 45 (minutes) * 60 (seconds)
                {
                    result = ts.Minutes + " minutes ago";
                }
                else if (delta < 5400) // 90 (minutes) * 60 (seconds)
                {
                    result = "an hour ago";
                }
                else if (delta < 86400) // 24 (hours) * 60 (minutes) * 60 (seconds)
                {
                    int hours = ts.Hours;
                    if (hours == 1)
                        hours = 2;
                    result = hours + " hours ago";
                }
                else if (delta < 172800) // 48 (hours) * 60 (minutes) * 60 (seconds)
                {
                    result = "yesterday";
                }
                else if (delta < 2592000) // 30 (days) * 24 (hours) * 60 (minutes) * 60 (seconds)
                {
                    result = ts.Days + " days ago";
                }
                else if (delta < 31104000) // 12 (months) * 30 (days) * 24 (hours) * 60 (minutes) * 60 (seconds)
                {
                    int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    result = months <= 1 ? "one month ago" : months + " months ago";
                }
                else
                {
                    int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    result = years <= 1 ? "one year ago" : years + " years ago";
                }
            }
            else
            {
                DateTime tmp1 = source;
                if (convertToUserTime)
                {
                    tmp1 = EngineContext.Current.Resolve<IDateTimeHelper>().ConvertToUserTime(tmp1, DateTimeKind.Utc);
                }
                //default formatting
                if (!String.IsNullOrEmpty(defaultFormat))
                {
                    result = tmp1.ToString(defaultFormat);
                }
                else
                {
                    result = tmp1.ToString();
                }
            }
            return result;
        }
    }


    public partial interface IDateTimeHelper
    {
        /// <summary>
        /// Retrieves a System.TimeZoneInfo object from the registry based on its identifier.
        /// </summary>
        /// <param name="id">The time zone identifier, which corresponds to the System.TimeZoneInfo.Id property.</param>
        /// <returns>A System.TimeZoneInfo object whose identifier is the value of the id parameter.</returns>
        TimeZoneInfo FindTimeZoneById(string id);

        /// <summary>
        /// Returns a sorted collection of all the time zones
        /// </summary>
        /// <returns>A read-only collection of System.TimeZoneInfo objects.</returns>
        ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones();

        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
        DateTime ConvertToUserTime(DateTime dt);

        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <param name="sourceDateTimeKind">The source datetimekind</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
        DateTime ConvertToUserTime(DateTime dt, DateTimeKind sourceDateTimeKind);

        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time to convert.</param>
        /// <param name="sourceTimeZone">The time zone of dateTime.</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
        DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone);

        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time to convert.</param>
        /// <param name="sourceTimeZone">The time zone of dateTime.</param>
        /// <param name="destinationTimeZone">The time zone to convert dateTime to.</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
        DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone);

        /// <summary>
        /// Converts the date and time to Coordinated Universal Time (UTC)
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
        DateTime ConvertToUtcTime(DateTime dt);

        /// <summary>
        /// Converts the date and time to Coordinated Universal Time (UTC)
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <param name="sourceDateTimeKind">The source datetimekind</param>
        /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
        DateTime ConvertToUtcTime(DateTime dt, DateTimeKind sourceDateTimeKind);

        /// <summary>
        /// Converts the date and time to Coordinated Universal Time (UTC)
        /// </summary>
        /// <param name="dt">The date and time to convert.</param>
        /// <param name="sourceTimeZone">The time zone of dateTime.</param>
        /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
        DateTime ConvertToUtcTime(DateTime dt, TimeZoneInfo sourceTimeZone);
        

        /// <summary>
        /// Gets or sets a default store time zone
        /// </summary>
        TimeZoneInfo DefaultStoreTimeZone { get; set; }

        /// <summary>
        /// Gets or sets the current user time zone
        /// </summary>
        TimeZoneInfo CurrentTimeZone { get; set; }
    }
}
