﻿using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Media.Projection;
using Android.Content;
using AutoLua.Droid.LuaScript.Utils.ScreenCaptures;
using AutoLua.Droid.Utils;

namespace AutoLua.Droid
{
    [Preserve(AllMembers = true)]
    [Register("AutoLua.Droid.MainActivity")]
    [Activity(Name = "AutoLua.Droid.MainActivity", Label = "AutoLua", Icon = "@mipmap/icon", Theme = "@style/MainTheme", LaunchMode = LaunchMode.SingleTask, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private const int RequestCodeMediaProjection = 17777;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            ScreenPermissions();
            StartService(new Intent(this, typeof(AutoNotificationService)));
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            MoveTaskToBack(true);
            ToastUtils.ShowToast("后台正常运行");
        }

        protected override void OnResume()
        {
            base.OnResume();
            var ipStr = AppUtils.GetIp();
            AutoNotificationService.Instance?.SetNotificationContentText($"IP {ipStr}:{AppApplication.HttpServerPort}");
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            //这事截屏的回调
            if (requestCode == RequestCodeMediaProjection && data != null)
            {
                InitScreenServer(data, resultCode);
            }
        }

        /// <summary>
        /// 初始化屏幕服务
        /// </summary>
        /// <param name="data"></param>
        /// <param name="resultCode"></param>
        private void InitScreenServer(Intent data, Result resultCode)
        {
            if (resultCode == Result.Ok)
            {
                ScreenCapturerServer.Instance.Init(data, this);
            }
        }


        /// <summary>
        /// 截屏权限申请
        /// </summary>
        private void ScreenPermissions()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                return;
            
            var mediaProjectionManager = AppApplication.GetSystemService<MediaProjectionManager>(MediaProjectionService);
            if (mediaProjectionManager != null)
            {
                StartActivityForResult(mediaProjectionManager.CreateScreenCaptureIntent(), RequestCodeMediaProjection);
            }
        }
    }
}