using System;

using Android.App;
using Android.Content;
using Android.Media.Projection;
using Android.OS;
using AutoLua.Droid.Utils;
using AutoLua.Droid.Utils.App;

namespace AutoLua.Droid.LuaScript.Utils.ScreenCaptures
{
    [Android.Runtime.Preserve(AllMembers = true)]
    [Activity(Theme = "@style/ScriptTheme.Transparent", ExcludeFromRecents = true)]
    public class ScreenCaptureRequestActivity : Activity
    {
        private const int RequestCodeMediaProjection = 17777;

        private static IScreenCaptureRequesterService _screenCaptureRequester;
        private Action<Result, Intent> _callback;

        public static void SetScreenCaptureRequesterService(IScreenCaptureRequesterService screenCaptureRequesterService)
        {
            _screenCaptureRequester = screenCaptureRequesterService;
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="callback"></param>
        public static void Request(Context context, Action<Result, Intent> callback)
        {
            var intent = new Intent(context, Java.Lang.Class.FromType(typeof(ScreenCaptureRequestActivity)))
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

            //请求权限。
            var mediaProjectionManager = AppApplication
                            .GetSystemService<MediaProjectionManager>(MediaProjectionService)
                            .CreateScreenCaptureIntent();

            StartActivityForResult(mediaProjectionManager, RequestCodeMediaProjection);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _callback = null;

            if (_screenCaptureRequester == null)
                return;

            _screenCaptureRequester?.Cancel();
            _screenCaptureRequester = null;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            ActivityEvenetManager.Instance.OnActivityResult(requestCode, resultCode, data);
            Finish();
        }
    }
}