using System;

namespace AutoLua.Droid.Expansions
{
    /// <summary>
    /// 对数字类型的扩展
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public static class NumberExpansion
    {
        #region 小数转时间

        /// <summary>
        /// 将秒转换成毫秒。
        /// </summary>
        /// <param name="second">秒数。</param>
        /// <returns>毫秒数。</returns>
        public static double GetSecondToMilliseconds(this double second)
        {
            return second * 1000;
        }

        /// <summary>
        /// 将分转换成毫秒。
        /// </summary>
        /// <param name="minute">分。</param>
        /// <returns>毫秒数。</returns>
        public static double GetMinuteToMilliseconds(this double minute)
        {
            return GetSecondToMilliseconds(60) * minute;
        }

        /// <summary>
        /// 将小时转换成毫秒。
        /// </summary>
        /// <param name="hour">小时。</param>
        /// <returns>毫秒数。</returns>
        public static double GetHourToMilliseconds(this double hour)
        {
            return GetMinuteToMilliseconds(60) * hour;
        }

        /// <summary>
        /// 将天转换成毫秒。
        /// </summary>
        /// <param name="day">天数。</param>
        /// <returns>毫秒数。</returns>
        public static double GetDayToMilliseconds(this double day)
        {
            return GetHourToMilliseconds(24) * day;
        }

        #endregion

        /// <summary>
        /// 将指定的时间与当前时间的间隔数转换成毫秒。
        /// </summary>
        /// <param name="executeTime">指定的时间。</param>
        /// <returns>毫秒数。</returns>
        public static double GetDateToMilliseconds(this DateTime executeTime)
        {
            if (executeTime <= DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(executeTime), "执行时间不可以是过去的时间");

            var span = executeTime - DateTime.Now;

            return span.TotalMilliseconds;
        }
    }
}