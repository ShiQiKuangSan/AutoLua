using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media.Projection;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using AutoLua.Core.Common;
using AutoLua.Core.LuaScript.ApiCommon.ScreenCaptures;
using AutoLua.Droid.Utils;
using AutoLua.Droid.Views.Loading;
using Com.Umeng.Analytics;
using System.IO;
using AndroidResource = Android.Resource;

namespace AutoLua.Droid.Base
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public abstract class BaseActivity : AppCompatActivity
    {
        public Toolbar CommonToolbar;
        protected Context Context;
        protected int statusBarColor = 0;
        protected View statusBarView;

        private CustomDialog _dialog;//进度条

        private const int RequestCodeMediaProjection = 1;
        private const int RequestCodeFile = 2;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(GetLayoutId());

            AppManager.Instance.AddActivity(this);

            if (statusBarColor == 0)
            {
                statusBarView =
                    StatusBarCompat.Compat(this, ContextCompat.GetColor(this, Resource.Color.colorPrimaryDark));
            }
            else if (statusBarColor != -1)
            {
                statusBarView = StatusBarCompat.Compat(this, statusBarColor);
            }

            Context = this;
            BindViews();
            CommonToolbar = FindViewById<Toolbar>(Resource.Id.common_toolbar);
            if (CommonToolbar != null)
            {
                InitToolBar();
                SetSupportActionBar(CommonToolbar);
            }

            InitData();
            ConfigViews();

            //申请截屏权限
            ScreenPermissions();
            FilePermissions();
        }

        /// <summary>
        /// 权限处理
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
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
        /// 权限处理
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="permissions"></param>
        /// <param name="grantResults"></param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            switch (requestCode)
            {
                case RequestCodeFile:
                    var path = AppApplication.LogPath;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    break;
            }
        }


        protected override void OnResume()
        {
            base.OnResume();
            MobclickAgent.OnResume(this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            MobclickAgent.OnPause(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            AppManager.Instance.FinishActivity(this);
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
                ScreenCapturerServer.Instance.Init(data);
            }
        }

        /// <summary>
        /// 截屏权限申请
        /// </summary>
        private void ScreenPermissions()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                return;

            var mediaProjectionManager = AppUtils.GetSystemService<MediaProjectionManager>(MediaProjectionService);
            if (mediaProjectionManager != null)
            {
                StartActivityForResult(mediaProjectionManager.CreateScreenCaptureIntent(), RequestCodeMediaProjection);
            }
        }

        private void FilePermissions()
        {
            RequestPermissions(new[] { Android.Manifest.Permission.WriteExternalStorage, Android.Manifest.Permission.ReadExternalStorage }, RequestCodeFile);
        }

        /// <summary>
        /// 获取布局
        /// </summary>
        /// <returns></returns>
        public abstract int GetLayoutId();

        /// <summary>
        /// 初始化工具栏
        /// </summary>
        public abstract void InitToolBar();

        /// <summary>
        /// 绑定视图
        /// </summary>
        public abstract void BindViews();

        /// <summary>
        /// 初始化数据。
        /// </summary>
        public abstract void InitData();

        /// <summary>
        /// 对各种控件进行设置、适配、填充数据
        /// </summary>
        public abstract void ConfigViews();

        protected void Gone(params View[] views)
        {
            if (views == null || views.Length <= 0)
                return;

            foreach (var view in views)
            {
                if (view != null)
                {
                    view.Visibility = ViewStates.Gone;
                }
            }
        }

        protected void Visible(params View[] views)
        {
            if (views == null || views.Length <= 0)
                return;

            foreach (var view in views)
            {
                if (view != null)
                {
                    view.Visibility = ViewStates.Visible;
                }
            }
        }

        protected bool IsVisible(View view)
        {
            return view.Visibility == ViewStates.Visible;
        }

        /// <summary>
        /// 获得一个弹窗实例
        /// </summary>
        /// <returns></returns>
        public CustomDialog GetDialog()
        {
            if (_dialog != null)
                return _dialog;

            _dialog = CustomDialog.Instance(this);
            _dialog.SetCancelable(true);
            return _dialog;
        }

        /// <summary>
        /// 隐藏弹窗
        /// </summary>
        public void HideDialog()
        {
            _dialog?.Hide();
        }

        /// <summary>
        /// 显示弹窗
        /// </summary>
        public void ShowDialog()
        {
            GetDialog().Show();
        }

        /// <summary>
        /// 释放弹窗
        /// </summary>
        public void DismissDialog()
        {
            if (_dialog != null)
            {
                _dialog.Dismiss();
                _dialog = null;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId != AndroidResource.Id.Home)
                return base.OnOptionsItemSelected(item);

            Finish();
            return true;
        }

        /// <summary>
        /// 隐藏状态栏
        /// </summary>
        protected void HideStatusBar()
        {
            var attrs = Window.Attributes;
            attrs.Flags |= WindowManagerFlags.Fullscreen;
            Window.Attributes = attrs;
            statusBarView?.SetBackgroundColor(Color.Transparent);
        }

        /// <summary>
        /// 显示状态栏
        /// </summary>
        protected void ShowStatusBar()
        {
            var attrs = Window.Attributes;
            attrs.Flags &= ~WindowManagerFlags.Fullscreen;
            Window.Attributes = attrs;
            statusBarView?.SetBackgroundColor(Color.Transparent);
            statusBarView?.SetBackgroundColor(new Color(statusBarColor));
        }
    }
}