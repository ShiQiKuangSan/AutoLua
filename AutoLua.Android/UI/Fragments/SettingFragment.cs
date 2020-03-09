using Android.OS;
using Android.Views;
using Android.Widget;
using AutoLua.Droid.Base;

namespace AutoLua.Droid.UI.Fragments
{
    public class SettingFragment : BaseFragment
    {
        private Switch _swtHttpServer;
        private Switch _swtAccessibility;

        public override int GetLayoutResId()
        {
            return Resource.Layout.activity_setting;
        }

        public override void BindViews(View view)
        {
            _swtHttpServer = view.FindViewById<Switch>(Resource.Id.swt_http_server);
            _swtAccessibility = view.FindViewById<Switch>(Resource.Id.swt_accessibility);
        }

        public override void InitData()
        {
        }

        public override void ConfigViews()
        {
        }

  
    }
}