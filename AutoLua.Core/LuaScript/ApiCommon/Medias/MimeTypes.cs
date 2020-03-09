using Android.Webkit;
using AutoLua.Core.LuaScript.Api;

namespace AutoLua.Core.LuaScript.ApiCommon.Medias
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class MimeTypes
    {
        public static string FromFile(string path)
        {
            var ext = LuaFiles.PFiles.GetExtension(path);

            return string.IsNullOrWhiteSpace(ext) ? "*/*" : MimeTypeMap.Singleton.GetMimeTypeFromExtension(ext);
        }

        public static string FromFileOr(string path, string defaultType)
        {
            var mimeType =  FromFile(path);
            return mimeType ?? defaultType;
        }
    }
}