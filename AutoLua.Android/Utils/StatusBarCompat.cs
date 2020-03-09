using Android.Annotation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using AndroidResource = Android.Resource;

namespace AutoLua.Droid.Utils
{
    /// <summary>
    /// 状态栏兼容
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public static class StatusBarCompat
    {
        private const int InvalidVal = -1;

        /// <summary>
        /// 兼容
        /// </summary>
        /// <param name="activity">视图</param>
        /// <param name="statusColor">状态颜色</param>
        /// <returns></returns>
        [TargetApi(Value = (int) BuildVersionCodes.Lollipop)]
        public static View Compat(Activity activity, int statusColor)
        {
            var color = ContextCompat.GetColor(activity, Resource.Color.colorPrimaryDark);
            
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                if (statusColor != InvalidVal)
                {
                    color = statusColor;
                }

                activity.Window.SetStatusBarColor(new Color(color));
                return null;
            }

            if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat || Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) 
                return null;
            
            var contentView = (ViewGroup) activity.FindViewById(AndroidResource.Id.Content);
            if (statusColor != InvalidVal)
            {
                color = statusColor;
            }

            var statusBarView = contentView.GetChildAt(0);
            if (statusBarView != null && statusBarView.MeasuredHeight == GetStatusBarHeight(activity))
            {
                statusBarView.SetBackgroundColor(new Color(color));
                return statusBarView;
            }

            statusBarView = new View(activity);

            var lp = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, GetStatusBarHeight(activity));
            statusBarView.SetBackgroundColor(new Color(color));
            contentView.AddView(statusBarView, lp);
            return statusBarView;

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        public static void Compat(Activity activity)
        {
            Compat(activity, InvalidVal);
        }

        /// <summary>
        /// 获得状态栏高度
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static int GetStatusBarHeight(Context context)
        {
            var result = 0;
            var resourceId = context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                result = context.Resources.GetDimensionPixelSize(resourceId);
            }

            return result;
        }
    }
}