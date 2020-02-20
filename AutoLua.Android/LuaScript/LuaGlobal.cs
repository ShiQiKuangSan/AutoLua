using System;
using AutoLua.Droid.AutoAccessibility.Accessibility.Filter;
using AutoLua.Droid.LuaScript.Api;
using AutoLua.Droid.LuaScript.Utils;
using System.IO;
using Android.AccessibilityServices;
using NLua.DynamicLua;
using Xamarin.Forms;

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
                }

                return _instance;
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
            
            //加载文件

            var assets = AppApplication.Instance.Assets;

            try
            {
                using var sr = new StreamReader(assets.Open("LuaJson.lua"));

                var str = sr.ReadToEnd();

                lua.LoadString(str);
            }
            catch (Exception)
            {
                AppApplication.OnLog("异常", "加载 json模块失败", Color.Red);
            }

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
            
            AppApplication.Lua = lua;
        }

        public void Close()
        {
            IsInit = false;
            AppApplication.Lua?.Dispose();
            AppApplication.LuaThread?.Interrupt();
            AppApplication.LuaThread = null;
        }
    }
}