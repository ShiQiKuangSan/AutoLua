using AutoLua.Droid.AutoAccessibility.Accessibility.Filter;
using AutoLua.Droid.LuaScript.Api;
using AutoLua.Droid.LuaScript.Utils;

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

            var lua = AppApplication.Lua;
            var dialogs = new Dialogs();

            //选择器
            lua["by"] = new By();
            lua["context"] = AppApplication.Instance.ApplicationContext;
            //app 操作
            lua["app"] = new LuaAppUtils(AppApplication.Instance);
            //设备信息
            lua["device"] = new Device();
            //弹窗
            lua["dialogs"] = dialogs;
            //脚本系统
            lua["engines"] = new Engines();
            //事件系统
            lua["events"] = new Api.Events();
            //文件系统
            lua["files"] = new LuaFiles();
            //网络请求模块
            lua["http"] = new HttpLua();
            //顔色模塊
            lua["colors"] = new Colors();
            lua["images"] = new Images();

            _luaGlobal = new LuaGlobalMethod(AppApplication.Instance);

            var luaGlobalType = typeof(LuaGlobalMethod);
            var dialogsType = typeof(Dialogs);

            //加载lua可以调用C#框架函数
            lua.LoadCLRPackage();
            //注册lua全局函数
            lua.RegisterFunction("sleep", _luaGlobal, luaGlobalType.GetMethod("Sleep"));
            lua.RegisterFunction("currentPackage", _luaGlobal, luaGlobalType.GetMethod("CurrentPackage"));
            lua.RegisterFunction("currentActivity", _luaGlobal, luaGlobalType.GetMethod("CurrentActivity"));
            lua.RegisterFunction("setClip", _luaGlobal, luaGlobalType.GetMethod("SetClip"));
            lua.RegisterFunction("getClip", _luaGlobal, luaGlobalType.GetMethod("GetClip"));
            lua.RegisterFunction("toast", _luaGlobal, luaGlobalType.GetMethod("Toast"));
            lua.RegisterFunction("toastLog", _luaGlobal, luaGlobalType.GetMethod("ToastLog"));
            lua.RegisterFunction("waitForActivity", _luaGlobal, luaGlobalType.GetMethod("WaitForActivity"));
            lua.RegisterFunction("waitForPackage", _luaGlobal, luaGlobalType.GetMethod("WaitForPackage"));
            lua.RegisterFunction("exit", _luaGlobal, luaGlobalType.GetMethod("Exit"));
            lua.RegisterFunction("print", _luaGlobal, luaGlobalType.GetMethod("Print"));
            lua.RegisterFunction("log", _luaGlobal, luaGlobalType.GetMethod("Print"));

            //点击
            lua.RegisterFunction("click", _luaGlobal, luaGlobalType.GetMethod("Click"));
            lua.RegisterFunction("longClick", _luaGlobal, luaGlobalType.GetMethod("LongClick"));
            lua.RegisterFunction("press", _luaGlobal, luaGlobalType.GetMethod("Press"));
            lua.RegisterFunction("swipe", _luaGlobal, luaGlobalType.GetMethod("Swipe"));
            lua.RegisterFunction("gesture", _luaGlobal, luaGlobalType.GetMethod("Gesture"));
            lua.RegisterFunction("gestures", _luaGlobal, luaGlobalType.GetMethod("Gestures"));

            //弹窗
            lua.RegisterFunction("alert", dialogsType, dialogsType.GetMethod("alert"));
        }
    }
}