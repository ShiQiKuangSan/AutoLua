using System;
using Android.App;
using Android.Content;

namespace AutoLua.Droid.LuaScript.Utils.ScreenCaptures
{
    /// <summary>
    /// 屏幕获取请求接口。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public interface IScreenCaptureRequesterService
    {
        /// <summary>
        /// 取消。
        /// </summary>
        void Cancel();

        /// <summary>
        /// 请求。
        /// </summary>
        void Request();

        /// <summary>
        /// 设置回调
        /// </summary>
        /// <param name="callback"></param>
        void SetResultCallback(Action<Result,Intent> callback);
    }
}