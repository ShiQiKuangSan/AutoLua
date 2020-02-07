using Android.Webkit;
using static AutoLua.Droid.LuaScript.Api.LuaFiles;

namespace AutoLua.Droid.LuaScript.Utils.Medias
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class MimeTypes
    {
        public static string FromFile(string path)
        {
            var ext = PFiles.GetExtension(path);

            return string.IsNullOrWhiteSpace(ext) ? "*/*" : MimeTypeMap.Singleton.GetMimeTypeFromExtension(ext);
        }

        public static string FromFileOr(string path, string defaultType)
        {
            var mimeType =  FromFile(path);
            return mimeType ?? defaultType;
        }
    }
}