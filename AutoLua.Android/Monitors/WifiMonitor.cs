using System;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.Runtime;
using AutoLua.Droid.Utils;

namespace AutoLua.Droid.Monitors
{
    /// <summary>
    /// wifi 监听
    /// </summary>
    internal class WifiMonitor : AbstractMonitor
    {
        private BroadcastReceiver receiver;

        public WifiMonitor(Context context) : base(context)
        {
        }

        public override void Init()
        {
            this.receiver = new WifiBroadcastReceiver();
        }

        [Obsolete]
#pragma warning disable CS0809 // 过时成员重写未过时成员
        public override void Register()
#pragma warning restore CS0809 // 过时成员重写未过时成员
        {
            var filter = new IntentFilter();
            filter.AddAction(WifiManager.NetworkIdsChangedAction);
            filter.AddAction(ConnectivityManager.ConnectivityAction);
            context.RegisterReceiver(receiver, filter);
        }

        public override void UnRegister()
        {
            if (receiver != null)
            {
                context.UnregisterReceiver(receiver);
            }
        }


        private class WifiBroadcastReceiver : BroadcastReceiver
        {
            [Obsolete]
#pragma warning disable CS0809 // 过时成员重写未过时成员
            public override void OnReceive(Context context, Intent intent)
#pragma warning restore CS0809 // 过时成员重写未过时成员
            {
                var ipStr = AppUtils.GetIp();
                context.JavaCast<AutoNotificationService>().SetNotificationContentText($"IP {ipStr}:{AppApplication.HttpServerPort}");
            }
        }
    }
}