using AutoLua.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoLua.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainNavPage : TabbedPage
    {
        private readonly IToastService toastService = DependencyService.Get<IToastService>();

        /// <summary>
        /// 最后一次点击退出按钮
        /// </summary>
        private DateTime backTimeOut = DateTime.Now;

        public MainNavPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            var result = DateTime.Now - backTimeOut;

            if (result.TotalMilliseconds < 2000)
            {
                return base.OnBackButtonPressed();
            }
            else
            {
                backTimeOut = DateTime.Now;
                toastService.ShortAlert("双击退出 AutoLua");
            }

            return true;
        }
    }
}