using Android.Content;

namespace AutoLua.Droid.Utils.Clip
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ClipboardUtil
    {
        private static ClipboardManager clipboardManager;

        public static Context Context { private get; set; }

        static ClipboardUtil()
        {
            clipboardManager = (ClipboardManager)Context?.GetSystemService(Context.ClipboardService);
        }

        /// <summary>
        /// 设置剪切板
        /// </summary>
        /// <param name="text"></param>
        public static void SetClip(string text)
        {
            clipboardManager.PrimaryClip = ClipData.NewPlainText("", text);
        }

        /// <summary>
        /// 获得剪切板
        /// </summary>
        /// <returns></returns>
        public static string GetClip()
        {
            var clip = clipboardManager.PrimaryClip;

            return (clip == null || clip.ItemCount == 0) ? string.Empty : clip.GetItemAt(0).Text ?? string.Empty;
        }
    }
}