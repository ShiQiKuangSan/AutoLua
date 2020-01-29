using AutoLua.Views;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AutoLua
{
    public partial class App : Application
    {
        public static Dictionary<string, Page> Pages { get; } = new Dictionary<string, Page>();

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainNavPage());
        }


        /// <summary>
        /// 获得页面实例。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public static T GetPage<T>(string page) where T : Page
        {
            if (!Pages.ContainsKey(page))
                return null;

            return Pages[page] as T;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
