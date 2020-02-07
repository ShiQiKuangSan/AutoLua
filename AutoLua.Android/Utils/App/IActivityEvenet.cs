using Android.App;
using Android.Content;

namespace AutoLua.Droid.Utils.App
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public interface IActivityEvenet
    {
        /// <summary>
        /// 活动回调
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
        void OnActivityResult(int requestCode, Result resultCode, Intent data);
    }
}