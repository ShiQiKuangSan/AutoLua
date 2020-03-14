using System;
using System.Collections.Generic;
using System.Linq;
using Android.Runtime;
using AutoLua.Droid.HttpServers.Models;
using AutoLua.Droid.Utils;
using HttpServer.Modules;
using Newtonsoft.Json.Linq;

namespace AutoLua.Droid.HttpServers.Controllers
{
    /// <summary>
    /// 节点控制器
    /// </summary>
    [Preserve(AllMembers = true)]
    public class NodeController : Controller
    {
        /// <summary>
        /// 节点点击
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/click")]
        public ActionResult Click(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.click-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                if (!node.Clickable)
                    return JsonSuccess(new { Status = false });

                //执行点击操作
                var status = node.Click();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }

        }

        /// <summary>
        /// 点击节点，如果节点不可点击则向上查找可点击的节点
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/clickParent")]
        public ActionResult ClickParent(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.clickParent-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                if (node.Clickable)
                {
                    //执行点击操作
                    var status = node.Click();

                    return JsonSuccess(new { Status = status });
                }
                else
                {
                    var parent = node.Parent();

                    if (parent == null)
                    {
                        return JsonSuccess(new { Status = false });
                    }

                    //遍历他的父节点
                    while (!parent.Clickable)
                    {
                        parent = parent.Parent();
                        if (parent == null)
                        {
                            return JsonSuccess(new { Status = false });
                        }
                    }

                    var status = parent.Click();

                    return JsonSuccess(new { Status = status });
                }
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }

        }

        /// <summary>
        /// 节点点击
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/longClick")]
        public ActionResult LongClick(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.longClick-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                if (!node.LongClickable)
                    return JsonSuccess(new { Status = false });

                //执行点击操作
                var status = node.LongClick();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/setText")]
        public ActionResult SetText(string key, string text)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.setText-- key is empty");

            try
            {
                text ??= string.Empty;

                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行设置文本操作
                var status = node.SetText(text);

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 节点复制
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/copy")]
        public ActionResult Copy(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.copy-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.Copy();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 对输入框文本的选中内容进行剪切，并返回是否操作成功。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/cut")]
        public ActionResult Cut(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.cut-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.Cut();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 对输入框文本的选中内容进行剪切，并返回是否操作成功。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/paste")]
        public ActionResult Paste(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.paste-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.Paste();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 设置选中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">选中的起始位置</param>
        /// <param name="end">选中的结束位置</param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/setSelection")]
        public ActionResult SetSelection(string key, int start, int end)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.setSelection-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.SetSelection(start, end);

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 对控件执行向前滑动的操作，并返回是否操作成功。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/scrollForward")]
        public ActionResult ScrollForward(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.scrollForward-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.ScrollForward();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 对控件执行向后滑动的操作，并返回是否操作成功。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/scrollBackward")]
        public ActionResult ScrollBackward(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.scrollBackward-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.ScrollBackward();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }


        /// <summary>
        /// 对控件执行"选中"操作，并返回是否操作成功。"选中"和Selected的属性相关，但该操作十分少用。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/select")]
        public ActionResult Select(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.select-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.Select();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }


        /// <summary>
        /// 对控件执行折叠操作，并返回是否操作成功
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/collapse")]
        public ActionResult Collapse(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.collapse-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.Collapse();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 对控件执行展开操作，并返回是否操作成功。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/expand")]
        public ActionResult Expand(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.expand-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.Expand();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }


        /// <summary>
        /// 对集合中所有控件执行显示操作，并返回是否全部操作成功。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/show")]
        public ActionResult Show(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.show-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.Show();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 对集合中所有控件执行向上滑的操作，并返回是否全部操作成功。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/scrollUp")]
        public ActionResult ScrollUp(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.scrollUp-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.ScrollUp();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 对集合中所有控件执行向下滑的操作，并返回是否全部操作成功。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/scrollDown")]
        public ActionResult ScrollDown(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.scrollDown-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.ScrollDown();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 对集合中所有控件执行向左滑的操作，并返回是否全部操作成功。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/scrollLeft")]
        public ActionResult ScrollLeft(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.scrollLeft-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.ScrollLeft();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 对集合中所有控件执行向右滑的操作，并返回是否全部操作成功。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/scrollRight")]
        public ActionResult ScrollRight(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.scrollRight-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess(new { Status = false });

                //执行辅助操作
                var status = node.ScrollRight();

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 返回该控件的所有子控件组成的控件集合。可以用于遍历一个控件的子控件
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/children")]
        public ActionResult Children(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.children-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                var list = new List<NodeModel>();

                if (node == null)
                    return JsonSuccess(list);

                for (var i = node.ChildCount - 1; i >= 0; i--)
                {
                    var child = node.Child(i);

                    if (child == null)
                        continue;

                    list.Add(child.To());
                }

                return JsonSuccess(list);
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 返回第i+1个子控件。如果i>=控件数目或者小于0
        /// </summary>
        /// <param name="key"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/child")]
        public ActionResult Child(string key, int i)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.child-- key is empty");

            if (i < 0)
                return JsonError("node.child-- i is mix 0");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess();

                var child = node.Child(i);

                return child == null ? JsonSuccess() : JsonSuccess(child.To());
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 返回该控件的父控件。如果该控件没有父控件，返回null。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/parent")]
        public ActionResult Parent(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.parent-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess();

                var parent = node.Parent();

                return parent == null ? JsonSuccess() : JsonSuccess(parent.To());
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 返回控件在屏幕上的范围，其值是一个Rect对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/bounds")]
        public ActionResult Bounds(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.bounds-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess();

                var bounds = node.Bounds();

                return JsonSuccess(new
                {
                    bounds.Left,
                    bounds.Top,
                    bounds.Right,
                    bounds.Bottom,
                    CenterX = bounds.CenterX(),
                    CenterY = bounds.CenterY(),
                    Width = bounds.Width(),
                    Height = bounds.Height()
                });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 返回控件在父控件中的范围，其值是一个Rect对象。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/boundsInParent")]
        public ActionResult BoundsInParent(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.boundsInParent-- key is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess();

                var bounds = node.BoundsInParent();

                return JsonSuccess(new
                {
                    bounds.Left,
                    bounds.Top,
                    bounds.Right,
                    bounds.Bottom,
                    CenterX = bounds.CenterX(),
                    CenterY = bounds.CenterY(),
                    Width = bounds.Width(),
                    Height = bounds.Height()
                });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 根据选择器selector在该控件的子控件、孙控件...中搜索符合该选择器条件的控件，并返回找到的第一个控件；如果没有找到符合条件的控件则返回null。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/findOne")]
        public ActionResult FindOne(string key, string json)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.findOne-- key is empty");

            if (string.IsNullOrWhiteSpace(json))
                return JsonError("node.findOne-- json is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                if (node == null)
                    return JsonSuccess();

                var obj = JObject.Parse(json);

                var by = NodeHelper.FilterJson(obj);

                var item = by.findOnce(node);

                return item == null ? JsonSuccess() : JsonSuccess(item.To());
            }
            catch (Exception e)
            {
                return JsonError($"json 格式错误 .. {e.Message}");
            }
        }

        /// <summary>
        /// 根据选择器selector在该控件的子控件、孙控件...中搜索符合该选择器条件的控件，并返回它们组合的集合。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/node/find")]
        public ActionResult Find(string key, string json)
        {
            if (string.IsNullOrWhiteSpace(key))
                return JsonError("node.find-- key is empty");

            if (string.IsNullOrWhiteSpace(json))
                return JsonError("node.find-- json is empty");

            try
            {
                var node = NodeHelper.GetCacheNode(key);

                var list = new List<NodeModel>();

                if (node == null)
                    return JsonSuccess(list);

                var obj = JObject.Parse(json);

                var by = NodeHelper.FilterJson(obj);

                var items = by.find(node);

                if (!items.Any())
                    return JsonSuccess(list);

                list = items.Select(x => x.To()).ToList();

                return JsonSuccess(list);
            }
            catch (Exception e)
            {
                return JsonError($"json 格式错误 .. {e.Message}");
            }
        }
    }
}