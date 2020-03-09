using System;
using System.IO;
using Android.AccessibilityServices;
using Android.Graphics;
using Android.Runtime;
using AutoLua.Core.AutoAccessibility;
using AutoLua.Core.AutoAccessibility.Accessibility.Node;
using AutoLua.Core.AutoAccessibility.Gesture;
using AutoLua.Core.LuaScript.ApiCommon.ScreenCaptures;
using AutoLua.Droid.HttpServers.Models;
using HttpServer.Modules;

namespace AutoLua.Droid.HttpServers.Controllers
{
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
                    return JsonError(mode, "没有截屏到");
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
                return JsonError(null, "未启动无障碍服务");
            }

            if (AutoAccessibilityService.Instance == null)
            {
                //未启动无障碍服务。
                AutoGlobal.Instance.GoToAccessibilitySetting();
                return JsonError(null, "未启动无障碍服务");
            }

            var baseNode = AutoAccessibilityService.Instance.RootInActiveWindow;

            if (baseNode == null)
            {
                return JsonError(null, "未获得节点");
            }

            var root = new UiNode(baseNode);
            var rect = root.Bounds();

            var model = new NodeModel
            {
                Index = root.Depth,
                ResourceId = root.FullId,
                Text = root.Text,
                Desc = root.Desc,
                ClassName = root.ClassName,
                Checkable = root.Checkable,
                Checked = root.Checked,
                Clickable = root.Clickable,
                Enabled = root.Enabled,
                Scrollable = root.Scrollable,
                LongClickable = root.LongClickable,
                Password = root.Password,
                Selected = root.IsSelected,
                Rect = new Models.Rect
                {
                    X = rect.Left,
                    Y = rect.Top,
                    Width = rect.Right - rect.Left,
                    Height = rect.Bottom - rect.Top,
                }
            };

            if (root.ChildCount > 0)
            {
                GetRoot(model, root);
            }

            return JsonSuccess(new
            {
                Activity = AutoGlobal.Instance.AccessibilityEvent.LatestActivity,
                Package = AutoGlobal.Instance.AccessibilityEvent.LatestPackage,
                Node = model
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
                return JsonError(null, "点击的坐标不能小于 0");
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
                return JsonError(null, "按键事件处理失败,参数key 为空");
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


        private static void GetRoot(NodeModel model, UiNode root)
        {
            for (var i = 0; i < root.ChildCount; i++)
            {
                var child = root.Child(i);

                if (child == null)
                    continue;

                if (!child.VisibleToUser)
                    continue;

                var rect = child.Bounds();

                var node = new NodeModel
                {
                    Index = child.Depth,
                    ResourceId = child.FullId,
                    Text = child.Text,
                    Desc = child.Desc,
                    ClassName = child.ClassName,
                    Checkable = child.Checkable,
                    Checked = child.Checked,
                    Clickable = child.Clickable,
                    Enabled = child.Enabled,
                    Scrollable = child.Scrollable,
                    LongClickable = child.LongClickable,
                    Password = child.Password,
                    Selected = child.IsSelected,
                    Rect = new Models.Rect
                    {
                        X = rect.Left,
                        Y = rect.Top,
                        Width = rect.Right - rect.Left,
                        Height = rect.Bottom - rect.Top,
                    }
                };

                model.Children.Add(node);

                if (child.ChildCount > 0)
                {
                    GetRoot(node, child);
                }
            }
        }
    }
}