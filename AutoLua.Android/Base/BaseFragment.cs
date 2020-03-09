using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using AutoLua.Droid.Views.Loading;

namespace AutoLua.Droid.Base
{
    public abstract class BaseFragment : Fragment
    {
        private CustomDialog dialog;
        
        /// <summary>
        /// 父视图
        /// </summary>
        protected View ParentView { get; private set; }

        protected new LayoutInflater LayoutInflater { get; private set; }

        /// <summary>
        /// 获得布局
        /// </summary>
        /// <returns></returns>
        public abstract int GetLayoutResId();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ParentView = inflater.Inflate(GetLayoutResId(), container, false);
            LayoutInflater = inflater;
            BindViews(ParentView);
            return ParentView;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            InitData();
            ConfigViews();
        }

        /// <summary>
        /// 绑定视图
        /// </summary>
        public abstract void BindViews(View view);


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

        protected bool IsViewVisible(View view)
        {
            return view.Visibility == ViewStates.Visible;
        }
        
        /// <summary>
        /// 获得一个弹窗实例
        /// </summary>
        /// <returns></returns>
        public CustomDialog GetDialog()
        {
            if (dialog != null) 
                return dialog;
            
            dialog = CustomDialog.Instance(Activity);
            dialog.SetCancelable(false);
            return dialog;
        }

        /// <summary>
        /// 隐藏弹窗
        /// </summary>
        public void HideDialog()
        {
            dialog?.Hide();
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
            if (dialog != null)
            {
                dialog.Dismiss();
                dialog = null;
            }
        }
    }
}