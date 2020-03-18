using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using AutoLua.Core.Logging;
using Java.Lang;
using Java.Util;

namespace AutoLua.Droid.Utils
{
    public sealed class AppManager
    {
        private static AppManager _instance = null;
        private static readonly object Lock = new object();

        public static AppManager Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (Lock)
                {
                    _instance = new AppManager();
                    return _instance;
                }
            }
        }

        private readonly List<Activity> activities = new List<Activity>();

        private AppManager()
        {

        }

        /// <summary>
        /// 添加Activity到堆栈
        /// </summary>
        /// <param name="activity"></param>
        public void AddActivity(Activity activity)
        {
            activities.Add(activity);
        }

        /// <summary>
        /// 获取当前Activity（堆栈中最后一个压入的）
        /// </summary>
        /// <returns></returns>
        public Activity CurrentActivity()
        {
            var activity = activities.Last();
            return activity;
        }

        /// <summary>
        /// 结束当前Activity（堆栈中最后一个压入的）
        /// </summary>
        public void FinishActivity()
        {
            var activity = activities.Last();
            FinishActivity(activity);
        }

        /// <summary>
        /// 结束指定的 activity
        /// </summary>
        /// <param name="activity"></param>
        public void FinishActivity(Activity activity)
        {
            if (activity != null)
            {
                activities.Remove(activity);
                activity.Finish();
            }
        }

        /// <summary>
        /// 结束指定的 activity
        /// </summary>
        /// <param name="activityType"></param>
        public void FinishActivity(Type activityType)
        {
            foreach (var activity in activities)
            {
                if (activities.GetType() == activityType)
                {
                    FinishActivity(activity);
                    break;
                }
            }
        }

        /// <summary>
        /// 结束所有Activity
        /// </summary>
        public void FinishAllActivity()
        {
            foreach (var activity in activities)
            {
                activity.Finish();
            }

            activities.Clear();
        }

        public void AppExit(Context context)
        {
            try
            {
                FinishAllActivity();
                var activityMgr = context.GetSystemService(Context.ActivityService).JavaCast<ActivityManager>();
                activityMgr.KillBackgroundProcesses(context.PackageName);
                JavaSystem.Exit(0);
            }
            catch (System.Exception e)
            {
                LoggerFactory.Current.Create().LogError(e.Message);
            }
        }
    }
}