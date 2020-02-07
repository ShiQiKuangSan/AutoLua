using System.Collections.Generic;
using Android.App;
using Android.Content;

namespace AutoLua.Droid.Utils.App
{
    /// <summary>
    /// 活动事件管理器。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ActivityEvenetManager
    {
        private static readonly object Lock = new object();

        private readonly List<IActivityEvenet> _delegates = new List<IActivityEvenet>();

        public void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            foreach (var item in _delegates)
            {
                item.OnActivityResult(requestCode, resultCode, data);
            }
        }

        public void AddDelegate(IActivityEvenet @delegate)
        {
            _delegates.Add(@delegate);
        }

        public void RemoveDelegate(IActivityEvenet @delegate)
        {
            _delegates.Remove(@delegate);
        }


        private static ActivityEvenetManager _instance;

        public static ActivityEvenetManager Instance
        {
            get
            {
                lock (Lock)
                {
                    _instance ??= new ActivityEvenetManager();
                }

                return _instance;
            }
        }
    }
}