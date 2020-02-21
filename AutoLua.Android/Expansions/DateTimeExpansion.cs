using System;

namespace AutoLua.Droid.Expansions
{
    /// <summary>
    /// 时间扩展。
    /// </summary>
    public static class DateTimeExpansion
    {
        /// <summary>
        /// 获得默认时间。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetDefaultTime(this DateTime time) => new DateTime(1970, 1, 1);

        /// <summary>
        /// 获得当前时间的时间戳（秒级）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetTimeStamp(this DateTime time)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// 获得当前时间的时间戳（毫秒级）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long GetTimeStampLong(this DateTime time)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            return (long)(time - startTime).TotalMilliseconds;
        }

        /// <summary>
        /// 获得去除时分秒后的当前时间。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetToday(this DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day);
        }

        /// <summary>
        /// 获得指定时间当天的开始时间和结束时间。
        /// </summary>
        /// <param name="time">当前时间</param>
        /// <returns>开始时间和结束时间</returns>
        public static DateTime[] GetTodayScope(this DateTime time)
        {
            var startTime = new DateTime(time.Year, time.Month, time.Day);
            var endTime = startTime.AddDays(1).AddSeconds(-1);

            return new[]
            {
                startTime,
                endTime
            };
        }
    }
}