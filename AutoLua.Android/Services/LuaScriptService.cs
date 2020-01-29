using AutoLua.Droid.LuaScript;
using AutoLua.Droid.Services;
using AutoLua.Droid.Utils;
using AutoLua.Events;
using AutoLua.Services;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(LuaScriptService))]
namespace AutoLua.Droid.Services
{
    public class LuaScriptService : ILuaScriptService
    {
        public object[] RunFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            return AppApplication.Lua?.DoFile(path);
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