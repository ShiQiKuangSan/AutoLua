using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Annotation;
using Android.Support.V4.App;
using Android.Util;
using AutoLua.Droid.AutoAccessibility;
using AutoLua.Droid.Expansions;
using AutoLua.Droid.Monitors;
using HttpServer.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoLua.Droid
{
    [Register("AutoLua.Droid.AutoNotificationService")]
    [Service(Name = "AutoLua.Droid.AutoNotificationService", Enabled = true, Exported = true)]
    [IntentFilter(new string[] { "Notification_Start", "Notification_Stop", }, Priority = 10000)]
    public class AutoNotificationService : Service
    {
        private NotificationCompat.Builder builder;
        private const int Notification_Id = 765;
        private readonly IList<AbstractMonitor> monitors = new List<AbstractMonitor>();
        private const string ActionStart = "Notification_Start";
        private const string ActionStop = "Notification_Stop";
        private const string TAG = "NotificationService";
        private AccessibilityHttpServer server;
        private IEnumerable<Type> assemblys = new List<Type>();

        public static AutoNotificationService Instance { get; private set; }

        public override void OnCreate()
        {
            base.OnCreate();
            Instance = this;

            var notificationIntent = new Intent(this, typeof(MainActivity));
            var channelId = "";
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                channelId = CreateNotificationChannel("AutoLua监控服务", "AutoLua监控服务");
            }

            builder = new NotificationCompat.Builder(this, channelId)
                .SetSmallIcon(Resource.Mipmap.icon)
                .SetTicker("AutoLua服务已启动")
                .SetContentTitle("Http服务")
                .SetContentText("IP")
                .SetContentIntent(PendingIntent.GetActivity(this, Notification_Id, notificationIntent, PendingIntentFlags.UpdateCurrent))
                .SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis());

            var notification = builder.Build();
            StartForeground(Notification_Id, notification);
            monitors.Add(new WifiMonitor(this));
            StartServer();
        }

        public void StartServer()
        {
            server?.Stop();
            server = null;
            server = new AccessibilityHttpServer(AppApplication.HttpServerPort);
            AssembliesRegister();
            Task.Factory.StartNew(() => server.Start());
        }

        /// <summary>
        /// 程序集注册控制器。
        /// </summary>
        private void AssembliesRegister()
        {
            if (assemblys.Any())
                return;

            //获取当前程序集。这是反射
            var assembly = Assembly.GetExecutingAssembly();

            //获取所有程序集
            //var assemblys = AppDomain.CurrentDomain.GetAssemblies();

            //遍历继承自 Controller 的类
            assemblys = assembly.GetTypes().Where(x => x.BaseType == typeof(Controller));

            assemblys.ForEach(x =>
            {
                //利用反射实例化类。
                var controller = Activator.CreateInstance(x) as Controller;
                server.RegisterController(controller);
            });
        }

        /// <summary>
        /// 设置通知栏信息
        /// </summary>
        /// <param name="text"></param>
        public void SetNotificationContentText(string text)
        {
            builder.SetContentText(text);

            var server = GetSystemService(NotificationService) as NotificationManager;

            server.Notify(Notification_Id, builder.Build());
        }

        /// <summary>
        /// 创建通知频道
        /// </summary>
        /// <param name="channelId">频道id</param>
        /// <param name="channelName">频道名称</param>
        /// <returns></returns>
        [RequiresApi(Api = (int)BuildVersionCodes.O)]
        private string CreateNotificationChannel(string channelId, string channelName)
        {
            var channel = new NotificationChannel(channelId, channelName, NotificationImportance.None)
            {
                LightColor = Color.Blue.ToArgb(),
                LockscreenVisibility = NotificationVisibility.Private
            };

            var service = AppApplication.GetSystemService<NotificationManager>(NotificationService);

            service.CreateNotificationChannel(channel);

            return channelId;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RemoveAllMonitor();
            StopForeground(true);
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            var action = intent.Action;

            if (ActionStart == action)
            {
                Log.Info(TAG, "接收启动服务操作，但忽略它");
            }
            else if (ActionStop == action)
            {
                StopSelf();
            }

            return StartCommandResult.NotSticky;
        }

        /// <summary>
        /// 移除所有监听。
        /// </summary>
        private void RemoveAllMonitor()
        {
            foreach (var item in monitors)
            {
                item.UnRegister();
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}