using System;
using Android.App;
using Android.Content;

namespace AutoLua.Droid.LuaScript.Utils.ScreenCaptures
{
    public abstract class BaseScreenCaptureRequester : IScreenCaptureRequester
    {
        protected Action<Result, Intent> CallBack;
        protected Intent Result;

        public void Cancel()
        {
            if (Result != null)
                return;

            CallBack?.Invoke((int) Android.App.Result.Canceled, null);
        }

        public abstract void Request();
        
        public void OnResult(Result resultCode, Intent data) {
            Result = data;
            CallBack?.Invoke(resultCode, data);
        }

        public virtual void SetOnActivityResultCallback(Action<Result, Intent> callback)
        {
            CallBack = callback;
        }
    }
}