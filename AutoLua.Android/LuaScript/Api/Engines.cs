using System;
using System.IO;
using Xamarin.Forms;

namespace AutoLua.Droid.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class Engines
    {
        public void execScriptFile(string file)
        {
            if (!File.Exists(file))
            {
                AppApplication.OnLog("错误", $"脚本 {file} 不存在", Color.Red);
                return;
            }

            try
            {
                AppApplication.Lua.LoadFile(file);
            }
            catch (Exception e)
            {
                AppApplication.OnLog("异常", e.Message, Color.Red);
            }
        }
    }
}