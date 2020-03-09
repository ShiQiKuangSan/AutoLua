using System;
using System.IO;
using AutoLua.Core.Common;

namespace AutoLua.Core.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class Engines
    {
        public void execScriptFile(string file)
        {
            if (!File.Exists(file))
            {
                //TODO 日志
                //AppApplication.OnLog("错误", $"脚本 {file} 不存在", Color.Red);
                return;
            }

            try
            {
                if (AppUtils.Lua != null)
                {
                    AppUtils.Lua.LoadFile(file);
                }
            }
            catch (Exception)
            {
                //TODO 日志
                //AppApplication.OnLog("异常", e.Message, Color.Red);
            }
        }
    }
}