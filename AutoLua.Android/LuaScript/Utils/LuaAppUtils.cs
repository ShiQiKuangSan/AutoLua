using Android.Content;
using Android.Content.PM;
using Java.Lang;
using Java.Net;
using Uri = Android.Net.Uri;

namespace AutoLua.Droid.LuaScript.Utils
{
    /// <summary>
    /// APP 工具类
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    internal class LuaAppUtils
    {
        private readonly Context _context;

        public LuaAppUtils(Context context)
        {
            _context = context;
        }

        /// <summary>
        /// 启动应用
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <returns>是否启动成功</returns>
        public bool launchApp(string appName)
        {
            var pkg = getPackageName(appName);
            return !string.IsNullOrWhiteSpace(pkg) && launchPackage(pkg);
        }

        /// <summary>
        /// 启动应用
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <returns>是否启动成功</returns>
        public bool launchPackage(string packageName)
        {
            try
            {
                var packageNameManager = _context.PackageManager;

                //关闭应用
                var clearTask = packageNameManager.GetLaunchIntentForPackage(packageName)
                    .AddFlags(ActivityFlags.ClearTask);

                _context.StartActivity(clearTask);

                //启动应用
                var newTask = packageNameManager.GetLaunchIntentForPackage(packageName)
                    .AddFlags(ActivityFlags.NewTask);

                _context.StartActivity(newTask);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获得包名
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <returns>包名</returns>
        public string getPackageName(string appName)
        {
            var packageManager = _context.PackageManager;

            var installedApplications = packageManager.GetInstalledApplications(PackageInfoFlags.MetaData);

            foreach (var applicationInfo in installedApplications)
            {
                var packageName = packageManager.GetApplicationLabel(applicationInfo);
                if (packageName == appName)
                {
                    return applicationInfo.PackageName;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 获得应用名称
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <returns>应用名称</returns>
        public string getAppName(string packageName)
        {
            var packageManager = _context.PackageManager;
            try
            {
                var applicationInfo = packageManager.GetApplicationInfo(packageName, 0);
                var appName = packageManager.GetApplicationLabel(applicationInfo);
                return appName;
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public bool goToAppDetailSettings(string packageName)
        {
            try
            {
                var i = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                i.AddCategory(Intent.CategoryDefault);
                i.AddFlags(ActivityFlags.NewTask);
                i.SetData(Uri.Parse($"package:{packageName}"));
                _context.StartActivity(i);
                return true;
            }
            catch (ActivityNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// 卸载应用
        /// </summary>
        /// <param name="packageName">包名</param>
        public void uninstall(string packageName)
        {
            var intent = new Intent(Intent.ActionDelete, Uri.Parse($"package:{packageName}"))
                .AddFlags(ActivityFlags.NewTask);
            _context.StartActivity(intent);
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <param name="url">连接</param>
        public void openUrl(string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = $"http://{url}";
            }

            var intent = new Intent(Intent.ActionView)
                .SetData(Uri.Parse(url))
                .AddFlags(ActivityFlags.NewTask);

            _context.StartActivity(intent);
        }


        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path"></param>
        public void viewFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new NullPointerException("path == null");

            IntentUtil.ViewFile(_context, path, null);
        }

        /// <summary>
        /// 编辑文件
        /// </summary>
        /// <param name="path"></param>
        public void editFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new NullPointerException("path == null");

            IntentUtil.EditFile(_context, path, null);
        }

        /// <summary>
        /// 解析uri字符串并返回相应的Uri对象。即使Uri格式错误，该函数也会返回一个Uri对象，但之后如果访问该对象的scheme, path等值可能因解析失败而返回null。
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public Uri parseUri(string uri)
        {
            if (uri.StartsWith("file://"))
                return getUriForFile(uri);

            return Uri.Parse(uri);
        }

        /// <summary>
        /// 从一个文件路径创建一个uri对象。
        /// 需要注意的是，在高版本Android上，由于系统限制直接在Uri暴露文件的绝对路径，因此返回的Uri会是诸如content://...的形式。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Uri getUriForFile(string path)
        {
            if (path.StartsWith("file://"))
            {
                path = path.Substring(7);
            }

            var file = new Java.IO.File(URI.Create(path));
            return Uri.FromFile(file);
        }

    }
}