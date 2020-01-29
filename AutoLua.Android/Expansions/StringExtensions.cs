namespace AutoLua.Droid.Expansions
{
    /// <summary>
    /// 字符串的扩展。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public static class StringExtensions
    {

        /// <summary>
        /// 获得字符串中文件格式的扩展名。
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static string GetExtension(this string sources)
        {
            var i = sources.LastIndexOf('.');
            if (i < 0 || i + 1 >= sources.Length - 1)
                return "";
            return sources.Substring(i + 1);
        }

        public static bool IsNullOrWhiteSpace(this string sources)
        {
            return string.IsNullOrWhiteSpace(sources);
        }
    }
}