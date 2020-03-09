using Android.Content;
using Android.Support.V4.Content;
using Android.Webkit;
using AutoLua.Core.Extensions;
using Java.IO;

namespace AutoLua.Core.LuaScript.ApiCommon.App
{
    [Android.Runtime.Preserve(AllMembers = true)]
    internal static class IntentUtil
    {
        public static bool ViewFile(Context context, string path, string fileProviderAuthority)
        {
            var ext = path.GetExtension();

            var mimeType = Android.Text.TextUtils.IsEmpty(ext) ? "*/*" : MimeTypeMap.Singleton.GetMimeTypeFromExtension(ext);
            return ViewFile(context, path, mimeType, fileProviderAuthority);
        }

        public static bool ViewFile(Context context, string path, string mimeType, string fileProviderAuthority)
        {
            try
            {
                var uri = GetUriOfFile(context, path, fileProviderAuthority);
                var intent = new Intent(Intent.ActionView)
                        .SetDataAndType(uri, mimeType)
                        .AddFlags(ActivityFlags.NewTask)
                        .AddFlags(ActivityFlags.GrantReadUriPermission)
                        .AddFlags(ActivityFlags.GrantWriteUriPermission);

                context.StartActivity(intent);
                return true;
            }
            catch (ActivityNotFoundException e)
            {
                e.PrintStackTrace();
                return false;
            }
        }


        public static bool EditFile(Context context, string path, string fileProviderAuthority)
        {
            try
            {
                var ext = path.GetExtension();

                var mimeType = Android.Text.TextUtils.IsEmpty(ext) ? "*/*" : MimeTypeMap.Singleton.GetMimeTypeFromExtension(ext);

                var uri = GetUriOfFile(context, path, fileProviderAuthority);
                var intent = new Intent(Intent.ActionEdit)
                        .SetDataAndType(uri, mimeType)
                        .AddFlags(ActivityFlags.NewTask)
                        .AddFlags(ActivityFlags.GrantReadUriPermission)
                        .AddFlags(ActivityFlags.GrantWriteUriPermission);

                context.StartActivity(intent);
                return true;
            }
            catch (ActivityNotFoundException e)
            {
                e.PrintStackTrace();
                return false;
            }
        }

        public static Android.Net.Uri GetUriOfFile(Context context, string path, string fileProviderAuthority)
        {
            Android.Net.Uri uri;
            if (fileProviderAuthority == null)
            {
                uri = Android.Net.Uri.Parse("file://" + path);
            }
            else
            {
                uri = FileProvider.GetUriForFile(context, fileProviderAuthority, new File(path));
            }
            return uri;
        }
    }
}