using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;

namespace AutoLua.Droid.Views.Loading
{
    [Register("AutoLua.Droid.Views.Loading.CustomDialog")]
    public class CustomDialog: Dialog
    {
        public CustomDialog(Context context, int themeResId = 0)
            :base(context, themeResId)
        {
        }

        public static CustomDialog Instance(Activity activity)
        {
            var v = (LoadingView)View.Inflate(activity, Resource.Layout.common_progress_view, null);
            
            v.SetColor(ContextCompat.GetColor(activity, Resource.Color.reader_menu_bg_color));
            
            var dialog = new CustomDialog(activity, Resource.Style.loading_dialog);
            
            dialog.SetContentView(v, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent)
            );
            
            return dialog;
        }
    }
}