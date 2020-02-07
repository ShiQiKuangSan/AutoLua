using AutoLua.Droid.Services;
using AutoLua.Services;
using NLua.Exceptions;
using System.Threading;
using Xamarin.Forms;

[assembly: Dependency(typeof(LuaScriptService))]
namespace AutoLua.Droid.Services
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class LuaScriptService : ILuaScriptService
    {
        public object[] RunFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            try
            {
                AppApplication.LuaThread = new Thread(() =>
                 {
                     try
                     {
                         AppApplication.Lua?.DoFile(path);
                     }
                     catch (LuaException e)
                     {
                         AppApplication.OnLog("异常", e.Message, Color.Red);
                     }
                 });

                AppApplication.LuaThread.Start();

                return null;
            }
            catch (LuaException e)
            {
                AppApplication.OnLog("异常", e.ToString(), Color.Red);
                return null;
            }
        }

        public object[] RunProject(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            ///这是一个文件夹，需要遍历里面的json文件

            return null;
        }
    }
}