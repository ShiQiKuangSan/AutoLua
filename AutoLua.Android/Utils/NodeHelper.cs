﻿using System;
using Android.Runtime;
using AutoLua.Core.AutoAccessibility.Accessibility.Filter;
using AutoLua.Core.AutoAccessibility.Accessibility.Node;
using AutoLua.Core.Caching;
using AutoLua.Droid.HttpServers.Models;
using Java.Util;
using Java.Util.Concurrent;
using Newtonsoft.Json.Linq;

namespace AutoLua.Droid.Utils
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public static class NodeHelper
    {
        /// <summary>
        /// 高并发的集合
        /// </summary>
        private static readonly ConcurrentHashMap uiObjects = new ConcurrentHashMap();

        /// <summary>
        /// 获得节点树，用于web页面显示节点树
        /// </summary>
        /// <param name="model"></param>
        /// <param name="root"></param>
        public static void GetRootTree(NodeModel model, UiNode root)
        {
            for (var i = 0; i < root.ChildCount; i++)
            {
                var child = root.Child(i);

                if (child == null)
                    continue;

                if (!child.VisibleToUser)
                    continue;

                var node = GetModel(child);

                model.Children.Add(node);

                if (child.ChildCount > 0)
                {
                    GetRootTree(node, child);
                }

                child.Recycle();
            }
        }

        /// <summary>
        /// 节点转换
        /// </summary>
        /// <param name="root"></param>
        /// <param name="isCache">是否缓存节点</param>
        /// <returns></returns>
        public static NodeModel To(this UiNode root, bool isCache = true)
        {
            var model = GetModel(root);

            if (isCache)
                AddCacheNode(model.Id, root);

            return model;
        }

        /// <summary>
        /// 转换节点类型
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private static NodeModel GetModel(UiNode root)
        {
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
                ChildCount = root.ChildCount,
                Rect = new Rect
                {
                    Left = rect.Left,
                    Top = rect.Top,
                    Right = rect.Right,
                    Bottom = rect.Bottom,
                    CenterX = rect.CenterX(),
                    CenterY = rect.CenterY(),
                    Width = rect.Right - rect.Left,
                    Height = rect.Bottom - rect.Top,
                }
            };

            return model;
        }


        /// <summary>
        /// json 转节点选择器
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static By FilterJson(JToken obj)
        {
            var by = new By();

            var id = obj.Value<string>("id");
            var idContains = obj.Value<string>("idContains");
            var idStartsWith = obj.Value<string>("idStartsWith");
            var idEndsWith = obj.Value<string>("idEndsWith");

            var text = obj.Value<string>("text");
            var textContains = obj.Value<string>("textContains");
            var textStartsWith = obj.Value<string>("textStartsWith");
            var textEndsWith = obj.Value<string>("textEndsWith");

            var desc = obj.Value<string>("desc");
            var descContains = obj.Value<string>("descContains");
            var descStartsWith = obj.Value<string>("descStartsWith");
            var descEndsWith = obj.Value<string>("descEndsWith");

            var textAndDesc = obj.Value<string>("textAndDesc");
            var textOrDesc = obj.Value<string>("textOrDesc");

            var textAndDescContains = obj.Value<string>("textAndDescContains");
            var textOrDescContains = obj.Value<string>("textOrDescContains");

            var textAndDescStartsWith = obj.Value<string>("textAndDescStartsWith");
            var textOrDescStartsWith = obj.Value<string>("textOrDescStartsWith");

            var textAndDescEndsWith = obj.Value<string>("textAndDescEndsWith");
            var textOrDescEndsWith = obj.Value<string>("textOrDescEndsWith");

            var className = obj.Value<string>("className");
            var classNameContains = obj.Value<string>("classNameContains");
            var classNameStartsWith = obj.Value<string>("classNameStartsWith");
            var classNameEndsWith = obj.Value<string>("classNameEndsWith");

            var checkable = obj.Value<string>("checkable");
            var checkeds = obj.Value<string>("checked");
            var clickable = obj.Value<string>("clickable");
            var enabled = obj.Value<string>("enabled");
            var scrollable = obj.Value<string>("scrollable");
            var longClickable = obj.Value<string>("longClickable");
            var password = obj.Value<string>("password");
            var selected = obj.Value<string>("selected");

            //包名
            var packageName = obj.Value<string>("packageName");

            //节点深度
            var depth = obj.Value<string>("depth");

            #region Id

            if (id != null)
            {
                by.id(id);
            }

            if (idContains != null)
            {
                by.idContains(idContains);
            }

            if (idStartsWith != null)
            {
                by.idStartsWith(idStartsWith);
            }

            if (idEndsWith != null)
            {
                by.idEndsWith(idEndsWith);
            }

            #endregion

            #region text

            if (text != null)
            {
                by.text(text);
            }

            if (textContains != null)
            {
                by.textContains(textContains);
            }

            if (textStartsWith != null)
            {
                by.textStartsWith(textStartsWith);
            }

            if (textEndsWith != null)
            {
                by.textEndsWith(textEndsWith);
            }

            #endregion

            #region desc

            if (desc != null)
            {
                by.desc(desc);
            }

            if (descContains != null)
            {
                by.descContains(descContains);
            }

            if (descStartsWith != null)
            {
                by.descStartsWith(descStartsWith);
            }

            if (descEndsWith != null)
            {
                by.descEndsWith(descEndsWith);
            }

            #endregion

            #region className

            if (className != null)
            {
                by.className(className);
            }

            if (classNameContains != null)
            {
                by.classNameContains(classNameContains);
            }

            if (classNameStartsWith != null)
            {
                by.classNameStartsWith(classNameStartsWith);
            }

            if (classNameEndsWith != null)
            {
                by.classNameEndsWith(classNameEndsWith);
            }

            #endregion

            if (textAndDesc != null)
            {
                by.filter(x => x.Text == textAndDesc && x.Desc == textAndDesc);
            }

            if (textOrDesc != null)
            {
                by.filter(x => x.Text == textOrDesc || x.Desc == textOrDesc);
            }

            if (checkable != null)
            {
                if (checkable.ToLower() == "true")
                {
                    by.checkable(true);
                }
                else if (checkable.ToLower() == "false")
                {
                    by.checkable(false);
                }
            }

            if (checkeds != null)
            {
                if (checkeds.ToLower() == "true")
                {
                    by.checkeds(true);
                }
                else if (checkeds.ToLower() == "false")
                {
                    by.checkeds(false);
                }
            }

            if (clickable != null)
            {
                if (clickable.ToLower() == "true")
                {
                    by.clickable(true);
                }
                else if (clickable.ToLower() == "false")
                {
                    by.clickable(false);
                }
            }

            if (enabled != null)
            {
                if (enabled.ToLower() == "true")
                {
                    by.enabled(true);
                }
                else if (enabled.ToLower() == "false")
                {
                    by.enabled(false);
                }
            }

            if (scrollable != null)
            {
                if (scrollable.ToLower() == "true")
                {
                    by.scrollable(true);
                }
                else if (scrollable.ToLower() == "false")
                {
                    by.scrollable(false);
                }
            }

            if (longClickable != null)
            {
                if (longClickable.ToLower() == "true")
                {
                    by.longClickable(true);
                }
                else if (longClickable.ToLower() == "false")
                {
                    by.longClickable(false);
                }
            }

            if (password != null)
            {
                if (password.ToLower() == "true")
                {
                    by.password(true);
                }
                else if (password.ToLower() == "false")
                {
                    by.password(false);
                }
            }

            if (selected != null)
            {
                if (selected.ToLower() == "true")
                {
                    by.selected(true);
                }
                else if (selected.ToLower() == "false")
                {
                    by.selected(false);
                }
            }

            if (packageName != null)
            {
                by.packageName(packageName);
            }

            if (textAndDescContains != null)
            {
                by.filter(x => x.Text.Contains(textAndDescContains) && x.Desc.Contains(textAndDescContains));
            }

            if (textOrDescContains != null)
            {
                by.filter(x => x.Text.Contains(textOrDescContains) || x.Desc.Contains(textOrDescContains));
            }

            if (textAndDescStartsWith != null)
            {
                by.filter(x => x.Text.StartsWith(textAndDescStartsWith) && x.Desc.StartsWith(textAndDescStartsWith));
            }

            if (textOrDescStartsWith != null)
            {
                by.filter(x => x.Text.StartsWith(textOrDescStartsWith) || x.Desc.StartsWith(textOrDescStartsWith));
            }

            if (textAndDescEndsWith != null)
            {
                by.filter(x => x.Text.EndsWith(textAndDescEndsWith) && x.Desc.EndsWith(textAndDescEndsWith));
            }

            if (textOrDescEndsWith != null)
            {
                by.filter(x => x.Text.EndsWith(textOrDescEndsWith) || x.Desc.EndsWith(textOrDescEndsWith));
            }

            if (depth != null)
            {
                var status = int.TryParse(depth, out var result);
                if (status)
                    by.filter(x => x.Depth == result);
            }

            return by;
        }


        /// <summary>
        /// 获得缓存节点
        /// </summary>
        /// <param name="key"></param>
        public static UiNode GetCacheNode(string key)
        {
            //添加节点缓存，默认2分钟
            var uinode = uiObjects.Get(key).JavaCast<UiNode>();
            return uinode;
        }

        /// <summary>
        /// 添加节点缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="model"></param>
        private static void AddCacheNode(string key, UiNode model)
        {
            //添加节点缓存，默认2分钟
            uiObjects.Put(key, model);
            var clearTimer = new Timer();
            //节点保存25秒。
            clearTimer.Schedule(new ClearUiObjectTimerTask(key), 20 * 1000);
        }

        private class ClearUiObjectTimerTask : TimerTask
        {
            private readonly string key;

            public ClearUiObjectTimerTask(string key)
            {
                this.key = key;
            }

            public override void Run()
            {
                uiObjects.Remove(key);
            }
        }
    }
}