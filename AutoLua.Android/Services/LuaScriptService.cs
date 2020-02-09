using AutoLua.Droid.Services;
using AutoLua.Services;
using NLua.Exceptions;
using System.Threading;
using AutoLua.Droid.LuaScript;
using Xamarin.Forms;

[assembly: Dependency(typeof(LuaScriptService))]
namespace AutoLua.Droid.Services
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class LuaScriptService : ILuaScriptService
    {
        public void RunFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            try
            {
                if (AppApplication.LuaThread != null)
                {
                    //关闭 lua脚本运行，关闭脚本线程
                    AppApplication.Lua.Close();
                    AppApplication.LuaThread.Interrupt();
                    AppApplication.LuaThread = null;
                }
                
                //执行lua脚本
                ScriptExecuteActivity.Execute(path);
            }
            catch (LuaException e)
            {
                AppApplication.OnLog("异常", e.ToString(), Color.Red);
            }
        }

        public void RunProject(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return ;

            //这是一个文件夹，需要遍历里面的json文件
        }
    }
}