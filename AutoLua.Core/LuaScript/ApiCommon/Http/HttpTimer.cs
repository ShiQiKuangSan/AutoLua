using System;

namespace AutoLua.Core.LuaScript.ApiCommon.Http
{
    public class HttpTimer
    {
        /// <summary>
        /// 获得时间戳。
        /// </summary>
        /// <returns></returns>
        public static long GetTime()
        {
            return GetUtcTime() - 28800000L;
        }

        public static long GetUtcTime()
        {
            return DateTime.UtcNow.Ticks / 10000L - 62135596800000L;
        }

        public static string Dt()
        {
            return string.Concat(new string[]
                                 {
                                     (DateTime.Now.Year - 1900).ToString(),
                                     (DateTime.Now.Month - 1).ToString(),
                                     DateTime.Now.Day.ToString(),
                                     DateTime.Now.Hour.ToString(),
                                     DateTime.Now.Minute.ToString(),
                                     DateTime.Now.Second.ToString(),
                                     (DateTime.Now.Millisecond % 100).ToString().PadLeft(2, '0')
                                 });
        }
    }
}