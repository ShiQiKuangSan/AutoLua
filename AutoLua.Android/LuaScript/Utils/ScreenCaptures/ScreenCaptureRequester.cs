using System;
using Android.App;
using Android.Content;
using AutoLua.Droid.Utils;
using AutoLua.Droid.Utils.App;

namespace AutoLua.Droid.LuaScript.Utils.ScreenCaptures
{
    public class ScreenCaptureRequester : BaseScreenCaptureRequester
    {
        public override void SetOnActivityResultCallback(Action<Result, Intent> callback)
        {
            base.SetOnActivityResultCallback((result, data) =>
            {
                Result = data;
                callback?.Invoke(result, data);
            });
        }


        public override void Request()
        {
            var activity = AppUtils.CurrentActivity;
            
            if (activity is IDelegateHost delegateHost)
            {
                var requester = new ActivityScreenCaptureRequester(delegateHost.GetOnActivityResultDelegateMediator(), activity);
                requester.SetOnActivityResultCallback(CallBack);
                requester.Request();
            }
            else
            {
                ScreenCaptureRequestActivity.Request(AppUtils.GetAppContext, CallBack);
            }
        }
    }
}