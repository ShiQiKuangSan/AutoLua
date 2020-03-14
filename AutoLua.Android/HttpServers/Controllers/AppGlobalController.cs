using Android.Runtime;
using AutoLua.Core.AutoAccessibility;
using HttpServer.Modules;

namespace AutoLua.Droid.HttpServers.Controllers
{
    /// <summary>
    /// http 全局信息控制器
    /// </summary>
    [Preserve(AllMembers = true)]
    public class AppGlobalController : Controller
    {
        /// <summary>
        /// 当前activity
        /// </summary>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/app/currentActivity")]
        public ActionResult CurrentActivity()
        {
            var status = AutoGlobal.Instance.IsAccessibilityServiceEnabled();

            if (!status)
            {
                //未启动无障碍服务。
                AutoGlobal.Instance.GoToAccessibilitySetting();
                return JsonError(null, "未启动无障碍服务");
            }

            if (AutoAccessibilityService.Instance == null)
            {
                //未启动无障碍服务。
                AutoGlobal.Instance.GoToAccessibilitySetting();
                return JsonError(null, "未启动无障碍服务");
            }

            var activity = AutoGlobal.Instance.AccessibilityEvent.LatestActivity;

            return JsonSuccess(activity);
        }

        /// <summary>
        /// 当前包名
        /// </summary>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/app/currentPackage")]
        public ActionResult CurrentPackage()
        {
            var status = AutoGlobal.Instance.IsAccessibilityServiceEnabled();

            if (!status)
            {
                //未启动无障碍服务。
                AutoGlobal.Instance.GoToAccessibilitySetting();
                return JsonError(null, "未启动无障碍服务");
            }

            if (AutoAccessibilityService.Instance == null)
            {
                //未启动无障碍服务。
                AutoGlobal.Instance.GoToAccessibilitySetting();
                return JsonError(null, "未启动无障碍服务");
            }

            var package = AutoGlobal.Instance.AccessibilityEvent.LatestPackage;

            return JsonSuccess(package);
        }
    }
}