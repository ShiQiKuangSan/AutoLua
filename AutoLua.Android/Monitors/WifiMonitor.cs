using System;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.Runtime;
using AutoLua.Core.Common;
using AutoLua.Droid.Services;

namespace AutoLua.Droid.Monitors
{
    /// <summary>
    /// wifi 监听
    /// </summary>
    internal class WifiMonitor : AbstractMonitor
    {
        private BroadcastReceiver _receiver;

        public WifiMonitor(Context context) : base(context)
        {
        }

        public override void Init()
        {
            _receiver = new WifiBroadcastReceiver();
        }

        [Obsolete]
#pragma warning disable CS0809 // 过时成员重写未过时成员
        public override void Register()
#pragma warning restore CS0809 // 过时成员重写未过时成员
        {
            var filter = new IntentFilter();
            filter.AddAction(WifiManager.NetworkIdsChangedAction);
            filter.AddAction(ConnectivityManager.ConnectivityAction);
            context.RegisterReceiver(_receiver, filter);
        }

        public override void UnRegister()
        {
            if (_receiver != null)
            {
                context.UnregisterReceiver(_receiver);
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