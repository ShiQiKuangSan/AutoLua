using System;
using System.IO;
using System.Linq;
using Android.AccessibilityServices;
using Android.Graphics;
using Android.Runtime;
using AutoLua.Core.AutoAccessibility;
using AutoLua.Core.AutoAccessibility.Accessibility.Node;
using AutoLua.Core.AutoAccessibility.Gesture;
using AutoLua.Core.LuaScript.ApiCommon.ScreenCaptures;
using AutoLua.Droid.HttpServers.Models;
using AutoLua.Droid.Utils;
using HttpServer.Modules;
using Newtonsoft.Json;

namespace AutoLua.Droid.HttpServers.Controllers
{
    /// <summary>
    /// 屏幕控制器
    /// </summary>
    [Preserve(AllMembers = true)]
    public class ScreenController : Controller
    {
        private readonly IGesture _gesture;

        public ScreenController()
        {
            _gesture = new DefatltGesture();
        }

        /// <summary>
        /// 截屏。
        /// </summary>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/screenshot")]
        public ActionResult GetScreen()
        {
            var mode = new ScreenModel();

            try
            {
                var bitmap = ScreenCapturerServerManager.Capturer();

                if (bitmap == null)
                {
                    return JsonError("没有截屏到");
                }

                using var outputStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 75, outputStream);
                bitmap.Recycle();
                var imageBytes = outputStream.ToArray();

                var base64 = Convert.ToBase64String(imageBytes);

                mode.Type = "jpeg";
                mode.Encoding = "base64";
                mode.Data = base64;

                return JsonSuccess(mode);
            }
            catch (Exception e)
            {
                return Html(e.Message);
            }
        }

        /// <summary>
        /// 获得节点层级。
        /// </summary>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/hierarchy")]
        public ActionResult GetHierarchy()
        {
            var status = AutoGlobal.Instance.IsAccessibilityServiceEnabled();

            if (!status)
            {
                //未启动无障碍服务。
                AutoGlobal.Instance.GoToAccessibilitySetting();
                return JsonError( "未启动无障碍服务");
            }

            if (AutoAccessibilityService.Instance == null)
            {
                //未启动无障碍服务。
                AutoGlobal.Instance.GoToAccessibilitySetting();
                return JsonError( "未启动无障碍服务");
            }

            var baseNode = AutoAccessibilityService.Instance.Windows.ToList();

            if (!baseNode.Any())
            {
                return JsonError("未获得节点");
            }

            var nodes = baseNode
                .Where(x => x.Root != null)
                .Select(x => new UiNode(x.Root))
                .Where(x=> x.VisibleToUser).ToList();
            
            var roots = new NodeModel();

            foreach (var root in nodes)
            {
                var model = root.To(false);
                
                if (root.ChildCount > 0)
                {
                    NodeHelper.GetRootTree(model, root);
                }

                roots.Children.Add(model);

                root.Recycle();
            }

            return JsonSuccess(new
            {
                Activity = AutoGlobal.Instance.AccessibilityEvent.LatestActivity,
                Package = AutoGlobal.Instance.AccessibilityEvent.LatestPackage,
                Node = roots
            });
        }

        /// <summary>
        /// 点击坐标
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(RouteMethod.POST, "/api/v1/click")]
        public ActionResult Click(ClickModel model)
        {
            if (model.X < 0 || model.Y < 0)
            {
                return JsonError("点击的坐标不能小于 0");
            }

            try
            {
                _gesture.Click(model.X, model.Y);
            }
            catch (Exception)
            {
                // ignored
            }

            return JsonSuccess();
        }

        [Route(RouteMethod.POST, "/api/v1/keyevent")]
        public ActionResult KeysEvent(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return JsonError("按键事件处理失败,参数key 为空");
            }

            switch (key)
            {
                case "home":
                    AutoAccessibilityService.Instance?.PerformGlobalAction(GlobalAction.Home);
                    break;
                case "back":
                    AutoAccessibilityService.Instance?.PerformGlobalAction(GlobalAction.Back);
                    break;
            }

            return JsonSuccess();
        }
    }
}