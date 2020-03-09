using System;
using System.IO;
using Android.AccessibilityServices;
using AutoLua.Core.AutoAccessibility.Accessibility.Filter;
using AutoLua.Core.Common;
using AutoLua.Core.LuaScript.Api;
using AutoLua.Core.LuaScript.ApiCommon.App;
using NLua.DynamicLua;

namespace AutoLua.Core.LuaScript
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
            _luaGlobal = new LuaGlobalMethod(AppUtils.AppContext);

            //元素选择器
            lua.by = new By();
            lua.context = AppUtils.AppContext;
            //app 操作
            lua.app = new LuaApp(AppUtils.AppContext);
            //设备信息
            lua.device = new Device();
            //弹窗
            lua.dialogs = dialogs;
            //脚本系统
            lua.engines = new Engines();
            //事件系统
            lua.events = new Events();
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

            AppUtils.Lua = lua;
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
                        //TODO 日志
                        //AppApplication.OnLog("异常", $"加载{file}模块失败", Color.Red);
                        continue;
                    }

                    lastIndex++;

                    var ex = file.Substring(lastIndex, file.Length - lastIndex);

                    if (ex.ToLower() != "lua")
                    {
                        //TODO 日志
                        //AppApplication.OnLog("异常", $"加载{file}模块失败", Color.Red);
                        continue;
                    }

                    var f = $"{path}/{file}";

                    using var sr = new StreamReader(AppUtils.GetAssets.Open(f));

                    var str = await sr.ReadToEndAsync();

                    lua.LoadString(str);
                }
                catch (Exception)
                {
                    //TODO 日志
                    //AppApplication.OnLog("异常", $"加载{file}模块失败", Color.Red);
                }
            }
        }

        public void Close()
        {
            IsInit = false;

            if (AppUtils.Lua != null)
            {
                AppUtils.Lua.threads?.exit();
                AppUtils.Lua.times?.Dispose();

                AppUtils.Lua.Dispose();
                AppUtils.Lua = null;
            }

            try
            {
                AppUtils.LuaThread?.Interrupt();
                AppUtils.LuaThread = null;
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}