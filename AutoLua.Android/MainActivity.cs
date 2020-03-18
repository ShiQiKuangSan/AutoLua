using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Content;
using Android.Support.V4.App;
using Android.Support.V4.View;
using AutoLua.Core.Common;
using AutoLua.Droid.Services;
using AutoLua.Droid.Base;
using AutoLua.Droid.UI.Fragments;
using AutoLua.Droid.Views;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using AutoLua.Core.Logging;

namespace AutoLua.Droid
{
    [Preserve(AllMembers = true)]
    [Register("AutoLua.Droid.MainActivity")]
    [Activity(Name = "AutoLua.Droid.MainActivity", Label = "AutoLua", Theme = "@style/AppTheme", Icon = "@mipmap/icon", LaunchMode = LaunchMode.SingleTask, MainLauncher = true,
        HardwareAccelerated = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : BaseActivity
    {
        /// <summary>
        /// 导航栏
        /// </summary>
        private RVPIndicator _indicator;

        /// <summary>
        /// 页面集合
        /// </summary>
        private ViewPager _viewPager;

        /// <summary>
        /// 底部导航栏的缓存集合
        /// </summary>
        private List<Fragment> _tabContents;

        /// <summary>
        /// Fragment 适配器
        /// </summary>
        private FragmentPagerAdapter _adapter;

        /// <summary>
        /// 标题集合
        /// </summary>
        private List<string> _titles;

        public override int GetLayoutId()
        {
            return Resource.Layout.activity_main;
        }

        public override void InitToolBar()
        {
            CommonToolbar.SetTitle(Resource.String.app_name);
        }

        public override void BindViews()
        {
            _indicator = FindViewById<RVPIndicator>(Resource.Id.indicator);
            _viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
        }

        public override void InitData()
        {
            //初始化http服务。
            StartService(new Intent(this, typeof(AutoNotificationService)));
            //LoggerFactory.Current.Create().LogError("测试日志");

            _titles = new List<string>
            {
                "脚本",
                "日志",
                "设置"
            };

            _tabContents = new List<Fragment>
            {
                new ScriptFragment(),
                new LogFragment(),
                new SettingFragment()
            };

            _adapter = new CustomFragmentPagerAdapter(SupportFragmentManager, _tabContents);
        }

        /// <summary>
        /// 对各种控件进行设置、适配、填充数据
        /// </summary>
        public override void ConfigViews()
        {
            _indicator.SetTabItemTitles(_titles);
            _viewPager.Adapter = _adapter;
            _viewPager.OffscreenPageLimit = 3;

            _indicator.SetViewPager(_viewPager, 0);
        }

        /// <summary>
        /// 后退
        /// </summary>
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        /// <summary>
        /// 自定义 Fragment 适配器
        /// </summary>
        private class CustomFragmentPagerAdapter : FragmentPagerAdapter
        {
            private readonly List<Fragment> _tabContents;

            public CustomFragmentPagerAdapter(FragmentManager fm, List<Fragment> tabContents) : base(fm)
            {
                _tabContents = tabContents;
            }

            public override int Count => _tabContents.Count;

            public override Fragment GetItem(int position)
            {
                return _tabContents[position];
            }
        }
    }
}