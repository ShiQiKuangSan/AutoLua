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
        private NotificationCompat.Builder _builder;
        private const int NotificationId = 765;
        private readonly IList<AbstractMonitor> _monitors = new List<AbstractMonitor>();
        private const string ActionStart = "Notification_Start";
        private const string ActionStop = "Notification_Stop";
        private const string Tag = "NotificationService";
        private AccessibilityHttpServer _server;
        private IEnumerable<Type> _controllers = new List<Type>();

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

            _builder = new NotificationCompat.Builder(this, channelId)
                .SetSmallIcon(Resource.Mipmap.icon)
                .SetTicker("AutoLua服务已启动")
                .SetContentTitle("Http服务")
                .SetContentText("IP")
                .SetContentIntent(PendingIntent.GetActivity(this, NotificationId, notificationIntent, PendingIntentFlags.UpdateCurrent))
                .SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis());

            var notification = _builder.Build();
            StartForeground(NotificationId, notification);
            _monitors.Add(new WifiMonitor(this));
            StartServer();
        }

        private void StartServer()
        {
            _server?.Stop();
            _server = null;
            _server = new AccessibilityHttpServer(AppApplication.HttpServerPort);
            AssembliesRegister();
            Task.Factory.StartNew(() => _server.Start());
        }

        /// <summary>
        /// 程序集注册控制器。
        /// </summary>
        private void AssembliesRegister()
        {
            if (_controllers.Any())
                return;

            //获取当前程序集。这是反射
            var assembly = Assembly.GetExecutingAssembly();

            //获取所有程序集
            //var assemblys = AppDomain.CurrentDomain.GetAssemblies();

            //遍历继承自 Controller 的类
            _controllers = assembly.GetTypes().Where(x => x.BaseType == typeof(Controller));

            _controllers.ForEach(x =>
            {
                //利用反射实例化类。
                var controller = Activator.CreateInstance(x) as Controller;
                _server.RegisterController(controller);
            });
        }

        /// <summary>
        /// 设置通知栏信息
        /// </summary>
        /// <param name="text"></param>
        public void SetNotificationContentText(string text)
        {
            _builder.SetContentText(text);

            var server = GetSystemService(NotificationService).JavaCast<NotificationManager>();

            server?.Notify(NotificationId, _builder.Build());
        }

        /// <summary>
        /// 创建通知频道
        /// </summary>
        /// <param name="channelId">频道id</param>
        /// <param name="channelName">频道名称</param>
        /// <returns></returns>
        [RequiresApi(Api = (int)BuildVersionCodes.O)]
        private static string CreateNotificationChannel(string channelId, string channelName)
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
                Log.Info(Tag, "接收启动服务操作，但忽略它");
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
            foreach (var item in _monitors)
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