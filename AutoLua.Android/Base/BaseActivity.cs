using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using AutoLua.Droid.Utils;
using AutoLua.Droid.Views.Loading;
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(GetLayoutId());
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