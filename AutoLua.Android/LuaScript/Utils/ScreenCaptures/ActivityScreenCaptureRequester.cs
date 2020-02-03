using Android.App;
using Android.Content;
using Android.Media.Projection;
using AutoLua.Droid.Utils.App;

namespace AutoLua.Droid.LuaScript.Utils.ScreenCaptures
{
    public class ActivityScreenCaptureRequester : BaseScreenCaptureRequester, IOnActivityResultDelegate
    {
        private const int RequestCodeMediaProjection = 17777;
        private readonly Mediator _mediator;
        private readonly Activity _activity;

        public ActivityScreenCaptureRequester(Mediator mediator, Activity activity)
        {
            _mediator = mediator;
            _activity = activity;
            _mediator.AddDelegate(RequestCodeMediaProjection, this);
        }

        public override void Request()
        {
            var mediaProjectionManager = AppApplication
                .GetSystemService<MediaProjectionManager>(Context.MediaProjectionService)
                .CreateScreenCaptureIntent();
            _activity.StartActivityForResult(mediaProjectionManager, RequestCodeMediaProjection);
        }

        public void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            Result = data;
            _mediator.RemoveDelegate(this);
            OnResult(resultCode, data);
        }
    }
}