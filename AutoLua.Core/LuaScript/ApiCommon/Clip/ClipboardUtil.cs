﻿using Android.Content;

namespace AutoLua.Core.LuaScript.ApiCommon.Clip
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public static class ClipboardUtil
    {
        private static readonly ClipboardManager ClipboardManager;

        public static Context Context { private get; set; }

        static ClipboardUtil()
        {
            ClipboardManager = (ClipboardManager)Context?.GetSystemService(Context.ClipboardService);
        }

        /// <summary>
        /// 设置剪切板
        /// </summary>
        /// <param name="text"></param>
        public static void SetClip(string text)
        {
            ClipboardManager.PrimaryClip = ClipData.NewPlainText("", text);
        }

        /// <summary>
        /// 获得剪切板
        /// </summary>
        /// <returns></returns>
        public static string GetClip()
        {
            var clip = ClipboardManager.PrimaryClip;

            return (clip == null || clip.ItemCount == 0) ? string.Empty : clip.GetItemAt(0).Text ?? string.Empty;
        }
    }
}