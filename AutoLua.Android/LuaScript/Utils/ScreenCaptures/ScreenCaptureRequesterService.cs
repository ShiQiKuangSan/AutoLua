using System;
using Android.App;
using Android.Content;
using AutoLua.Droid.Utils;
using AutoLua.Droid.Utils.App;

namespace AutoLua.Droid.LuaScript.Utils.ScreenCaptures
{
    /// <summary>
    /// 屏幕获取请求接口。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ScreenCaptureRequesterService : IScreenCaptureRequesterService, IActivityEvenet
    {
        protected Action<Result, Intent> CallBack;
        protected Intent Result;

        /// <summary>
        /// 取消截屏
        /// </summary>
        public void Cancel()
        {
            if (Result != null)
                return;

            CallBack?.Invoke(Android.App.Result.Canceled, null);
#pragma warning disable CS0618 // 类型或成员已过时
            AppApplication.LuaThread?.Resume();
#pragma warning restore CS0618 // 类型或成员已过时
        }

        public void Request()
        {
            var activity = AppUtils.CurrentActivity;

            if (activity == null)
                return;

            ScreenCaptureRequestActivity.SetScreenCaptureRequesterService(this);
            ScreenCaptureRequestActivity.Request(activity, CallBack);
#pragma warning disable CS0618 // 类型或成员已过时
            AppApplication.LuaThread?.Suspend();
#pragma warning restore CS0618 // 类型或成员已过时
        }

        /// <summary>
        /// 设置回调
        /// </summary>
        /// <param name="callback"></param>
        public void SetResultCallback(Action<Result, Intent> callback)
        {
            this.CallBack = callback;
        }

        /// <summary>
        /// 权限请求后的回调。
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
        public void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            Result = data;
            CallBack?.Invoke(resultCode, data);
#pragma warning disable CS0618 // 类型或成员已过时
            AppApplication.LuaThread?.Resume();
#pragma warning restore CS0618 // 类型或成员已过时
        }
    }
}