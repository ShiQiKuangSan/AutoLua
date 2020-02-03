using System;
using Android.App;
using Android.Content;
using Android.OS;
using AutoLua.Droid.LuaScript.Utils.ScreenCaptures;
using AutoLua.Droid.Utils;
using AutoLua.Droid.Utils.App;
using Java.Lang;

namespace AutoLua.Droid
{
    [Activity(Theme = "@style/ScriptTheme.Transparent", ExcludeFromRecents = true)]
    public class ScreenCaptureRequestActivity : Activity
    {
        private readonly Mediator _onActivityResultDelegateMediator = new Mediator();
        private IScreenCaptureRequester _screenCaptureRequester;
        private Action<Result, Intent> _callback;

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="callback"></param>
        public static void Request(Context context, Action<Result, Intent> callback) {
            var intent = new Intent(context, Class.FromType(typeof(ScreenCaptureRequestActivity)))
                .AddFlags(ActivityFlags.NewTask);
            
            IntentExtras.NewExtras()
                .Put("callback", callback)
                .PutInIntent(intent);
            
            context.StartActivity(intent);
        }
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var extras = IntentExtras.FromIntentAndRelease(Intent);
            if (extras == null)
            {
                Finish();
                return;
            }

            _callback = extras.Get<Action<Result, Intent>>("callback");
            if (_callback == null)
            {
                Finish();
                return;
            }

            _screenCaptureRequester = new ActivityScreenCaptureRequester(_onActivityResultDelegateMediator, this);
            _screenCaptureRequester.SetOnActivityResultCallback(_callback);
            _screenCaptureRequester.Request();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _callback = null;
            
            if (_screenCaptureRequester == null)
                return;
            
            _screenCaptureRequester.Cancel();
            _screenCaptureRequester = null;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            _onActivityResultDelegateMediator.OnActivityResult(requestCode, resultCode, data);
            Finish();
        }
    }
}