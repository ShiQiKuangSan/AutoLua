using System;
using AutoLua.Droid.AutoAccessibility.Accessibility.Filter;
using AutoLua.Droid.LuaScript.Api;
using AutoLua.Droid.LuaScript.Utils;
using System.IO;
using Android.AccessibilityServices;
using NLua.DynamicLua;
using Xamarin.Forms;
using AutoLua.Droid.Utils;

namespace AutoLua.Droid.LuaScript
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class LuaGlobal
    {
        private bool IsInit { get; set; }

        private LuaGlobalMethod _luaGlobal;

        private static LuaGlobal _instance;

        private static readonly object Lock = new object();

        public static LuaGlobal Instance
        {
            get
            {
                lock (Lock)
                {
                    _instance ??= new LuaGlobal();
                    return _instance;
                }
            }
        }

        public void Init()
        {
            if (IsInit)
                return;

            IsInit = true;

            dynamic lua = new DynamicLua();

            var dialogs = new Dialogs();
            _luaGlobal = new LuaGlobalMethod(AppApplication.Instance);

            //元素选择器
            lua.by = new By();
            lua.context = AppApplication.Instance.ApplicationContext;
            //app 操作
            lua.app = new LuaAppUtils(AppApplication.Instance);
            //设备信息
            lua.device = new Api.Device();
            //弹窗
            lua.dialogs = dialogs;
            //脚本系统
            lua.engines = new Engines();
            //事件系统
            lua.events = new Api.Events();
            //文件系统
            lua.files = new LuaFiles();
            //网络请求模块
            lua.http = new HttpLua();
            //顔色模塊
            lua.colors = new Colors();
            //找图找色模块
            lua.images = new Images();
            //无障碍服务手势
            lua.keys = new KeysApi();
            //多媒体
            lua.media = new Media();
            //传感器
            lua.sensors = new Sensors();
            //本地存储
            lua.storages = new SharedPreferences();
            //线程模块
            lua.threads = new ThreadApi();
            var time = new TimerApi();
            //定时器模块
            lua.times = time;

            //注册lua全局函数
            lua.sleep = new Action<int>(_luaGlobal.Sleep);
            lua.currentPackage = new Func<string>(_luaGlobal.CurrentPackage);
            lua.currentActivity = new Func<string>(_luaGlobal.CurrentActivity);
            lua.setClip = new Action<string>(_luaGlobal.SetClip);
            lua.getClip = new Func<string>(_luaGlobal.GetClip);
            lua.toast = new Action<string>(_luaGlobal.Toast);
            lua.toastLog = new Action<string>(_luaGlobal.ToastLog);
            lua.waitForActivity = new Action<string, int>(_luaGlobal.WaitForActivity);
            lua.waitForPackage = new Action<string, int>(_luaGlobal.WaitForPackage);
            lua.exit = new Action(_luaGlobal.Exit);
            lua.print = new Action<object>(_luaGlobal.Print);
            lua.log = new Action<object>(_luaGlobal.Print);

            //点击
            lua.click = new Func<int, int, bool>(_luaGlobal.Click);
            lua.longClick = new Func<int, int, bool>(_luaGlobal.LongClick);
            lua.press = new Func<int, int, int, bool>(_luaGlobal.Press);
            lua.swipe = new Func<int, int, int, int, int, bool>(_luaGlobal.Swipe);
            lua.gesture = new Func<int, int[][], bool>(_luaGlobal.Gesture);
            lua.gestures = new Func<GestureDescription.StrokeDescription[], bool>(_luaGlobal.Gestures);

            //弹窗
            lua.alert = new Action<string, string, Action>(dialogs.alert);

            //定时器
            lua.setInterval = new Func<Action, long, string>(time.setInterval);
            lua.setTimeout = new Action<Action, long>(time.setTimeout);

            //加载文件

            LoadScript(lua);

            AppApplication.Lua = lua;
        }


        private static async void LoadScript(dynamic lua)
        {
            var path = "Script";
            var scripts = AppUtils.GetAssets.List(path);

            foreach (var file in scripts)
            {
                try
                {
                    var lastIndex = file.LastIndexOf('.');

                    if (lastIndex <= -1)
                    {
                        AppApplication.OnLog("异常", $"加载{file}模块失败", Color.Red);
                        continue;
                    }

                    lastIndex++;

                    var ex = file.Substring(lastIndex, file.Length - lastIndex);

                    if (ex.ToLower() != "lua")
                    {
                        AppApplication.OnLog("异常", $"加载{file}模块失败", Color.Red);
                        continue;
                    }

                    var f = $"{path}/{file}";

                    using var sr = new StreamReader(AppUtils.GetAssets.Open(f));

                    var str = await sr.ReadToEndAsync();

                    lua.LoadString(str);
                }
                catch (Exception)
                {
                    AppApplication.OnLog("异常", $"加载{file}模块失败", Color.Red);
                }
            }
        }

        public void Close()
        {
            IsInit = false;

            if (AppApplication.Lua != null)
            {
                AppApplication.Lua.threads?.exit();
                AppApplication.Lua.times?.Dispose();

                AppApplication.Lua.Dispose();
                AppApplication.Lua = null;
            }

            try
            {

                AppApplication.LuaThread?.Interrupt();
                AppApplication.LuaThread = null;
            }
            catch (Exception)
            {
            }
        }
    }
}