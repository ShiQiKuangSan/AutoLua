using AutoLua.Droid.AutoAccessibility.Accessibility.Filter;
using AutoLua.Droid.LuaScript.Api;
using AutoLua.Droid.LuaScript.Utils;

namespace AutoLua.Droid.LuaScript
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class LuaGlobal
    {
        private bool IsInit { get; set; }

        private LuaGlobalMethod luaGlobal;

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

            lua["engines"] = new Engines();
            lua["events"] = new Api.Events();

            luaGlobal = new LuaGlobalMethod(AppApplication.Instance);

            var luaGlobalType = typeof(LuaGlobalMethod);
            var dialogsType = typeof(Dialogs);

            //加载lua可以调用C#框架函数
            lua.LoadCLRPackage();
            //注册lua全局函数
            lua.RegisterFunction("sleep", luaGlobal, luaGlobalType.GetMethod("Sleep"));
            lua.RegisterFunction("currentPackage", luaGlobal, luaGlobalType.GetMethod("CurrentPackage"));
            lua.RegisterFunction("currentActivity", luaGlobal, luaGlobalType.GetMethod("CurrentActivity"));
            lua.RegisterFunction("setClip", luaGlobal, luaGlobalType.GetMethod("SetClip"));
            lua.RegisterFunction("getClip", luaGlobal, luaGlobalType.GetMethod("GetClip"));
            lua.RegisterFunction("toast", luaGlobal, luaGlobalType.GetMethod("Toast"));
            lua.RegisterFunction("toastLog", luaGlobal, luaGlobalType.GetMethod("ToastLog"));
            lua.RegisterFunction("waitForActivity", luaGlobal, luaGlobalType.GetMethod("WaitForActivity"));
            lua.RegisterFunction("waitForPackage", luaGlobal, luaGlobalType.GetMethod("WaitForPackage"));
            lua.RegisterFunction("exit", luaGlobal, luaGlobalType.GetMethod("Exit"));
            lua.RegisterFunction("print", luaGlobal, luaGlobalType.GetMethod("Print"));
            lua.RegisterFunction("log", luaGlobal, luaGlobalType.GetMethod("Print"));

            //点击
            lua.RegisterFunction("click", luaGlobal, luaGlobalType.GetMethod("Click"));
            lua.RegisterFunction("longClick", luaGlobal, luaGlobalType.GetMethod("LongClick"));
            lua.RegisterFunction("press", luaGlobal, luaGlobalType.GetMethod("Press"));
            lua.RegisterFunction("swipe", luaGlobal, luaGlobalType.GetMethod("Swipe"));
            lua.RegisterFunction("gesture", luaGlobal, luaGlobalType.GetMethod("Gesture"));
            lua.RegisterFunction("gestures", luaGlobal, luaGlobalType.GetMethod("Gestures"));

            //弹窗
            lua.RegisterFunction("alert", dialogsType, dialogsType.GetMethod("alert"));
        }
    }
}