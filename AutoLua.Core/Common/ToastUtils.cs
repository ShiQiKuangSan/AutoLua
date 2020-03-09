using Android.Content;
using Android.Widget;

namespace AutoLua.Core.Common
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public static class ToastUtils
    {
        private static Toast _toast;
        private static readonly Context Context = AppUtils.AppContext;

        /********************** 非连续弹出的Toast ***********************/
        public static void ShowSingleToast(int resId)
        { //R.string.**
            GetSingleToast(resId, ToastLength.Short).Show();
        }

        public static void ShowSingleToast(string text)
        {
            GetSingleToast(text, ToastLength.Short).Show();
        }

        public static void ShowSingleLongToast(int resId)
        {
            GetSingleToast(resId, ToastLength.Long).Show();
        }

        public static void ShowSingleLongToast(string text)
        {
            GetSingleToast(text, ToastLength.Long).Show();
        }

        /*********************** 连续弹出的Toast ************************/
        public static void ShowToast(int resId)
        {
            GetToast(resId, ToastLength.Short).Show();
        }

        public static void ShowToast(string text)
        {
            GetToast(text, ToastLength.Short).Show();
        }

        public static void ShowLongToast(int resId)
        {
            GetToast(resId, ToastLength.Long).Show();
        }

        public static void ShowLongToast(string text)
        {
            GetToast(text, ToastLength.Long).Show();
        }

        private static Toast GetSingleToast(int resId, ToastLength duration)
        { // 连续调用不会连续弹出，只是替换文本
            return GetSingleToast(Context.Resources.GetText(resId), duration);
        }

        private static Toast GetSingleToast(string text, ToastLength duration)
        {
            if (_toast == null)
            {
                _toast = Toast.MakeText(Context, text, duration);
            }
            else
            {
                _toast.SetText(text);
            }
            return _toast;
        }

        private static Toast GetToast(int resId, ToastLength duration)
        { // 连续调用会连续弹出
            return GetToast(Context.Resources.GetText(resId), duration);
        }

        private static Toast GetToast(string text, ToastLength duration)
        {
            return Toast.MakeText(Context, text, duration);
        }
    }
}