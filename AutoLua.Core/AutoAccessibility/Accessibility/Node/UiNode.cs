﻿using System;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View.Accessibility;

namespace AutoLua.Core.AutoAccessibility.Accessibility.Node
{
    [Preserve(AllMembers = true)]
    public class UiNode : AccessibilityNodeInfoCompat
    {
        #region 初始化

#pragma warning disable CS0618 // 类型或成员已过时
        public UiNode(Java.Lang.Object info, int depth = 0, int indexInParent = -1) : base(info)
#pragma warning restore CS0618 // 类型或成员已过时
        {
            Depth = depth;
            IndexInParent = indexInParent;
        }

        #endregion

        #region 字段

        /// <inheritdoc cref="Depth" />
        /// <summary>
        /// 节点深度。
        /// </summary>
        public int Depth { get; }

        /// <inheritdoc cref="IndexInParent" />
        /// <summary>
        /// 节点下标
        /// </summary>
        public int IndexInParent { get; }

        /// <inheritdoc cref="Clickable" />
        /// <summary>
        /// 是否可以点击
        /// </summary>
        public new bool Clickable => base.Clickable;

        /// <inheritdoc cref="IsSelected" />
        /// <summary>
        /// 是否选中。
        /// </summary>
        public bool IsSelected => base.Selected;

        /// <inheritdoc cref="FullId" />
        /// <summary>
        /// 节点的完整编号。
        /// </summary>
        public string FullId => ViewIdResourceName ?? string.Empty;

        /// <inheritdoc cref="ClassName" />
        /// <summary>
        /// 节点的类名。
        /// </summary>
        public new string ClassName => base.ClassName ?? string.Empty;

        /// <summary>
        /// 节点文本。
        /// </summary>
        public new string Text => base.Text ?? string.Empty;

        /// <inheritdoc cref="Desc" />
        /// <summary>
        /// 节点详细文本。
        /// </summary>
        public string Desc => ContentDescription ?? string.Empty;

        /// <inheritdoc cref="ChildCount" />
        /// <summary>
        /// 当前节点的子节点数。
        /// </summary>
        public new int ChildCount => base.ChildCount;

        #endregion

        #region 控件基础操作

        /// <summary>
        /// 点击控件
        /// </summary>
        /// <returns></returns>
        public bool Click()
        {
            return PerformAction(ActionClick);
        }

        /// <summary>
        /// 长按控件
        /// </summary>
        /// <returns></returns>
        public bool LongClick()
        {
            return PerformAction(ActionLongClick);
        }

        /// <summary>
        /// 获取此节点是否为可访问性焦点。
        /// </summary>
        public bool AccessibilityFocus()
        {
            return PerformAction(ActionAccessibilityFocus);
        }

        /// <summary>
        /// 清除此节点是否为可访问性焦点。
        /// </summary>
        public bool ClearAccessibilityFocus()
        {
            return PerformAction(ActionClearAccessibilityFocus);
        }

        /// <summary>
        /// 置入焦点。
        /// </summary>
        public bool Focus()
        {
            return PerformAction(ActionFocus);
        }

        /// <summary>
        /// 清除焦点
        /// </summary>
        /// <returns></returns>
        public bool ClearFocus()
        {
            return PerformAction(ActionClearFocus);
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public bool Copy()
        {
            return PerformAction(ActionCopy);
        }

        /// <summary>
        /// 粘贴
        /// </summary>
        /// <returns></returns>
        public bool Paste()
        {
            return PerformAction(ActionPaste);
        }

        /// <summary>
        /// 选中。
        /// </summary>
        /// <returns></returns>
        public bool Select()
        {
            return PerformAction(ActionSelect);
        }

        /// <summary>
        /// 剪切
        /// </summary>
        /// <returns></returns>
        public bool Cut()
        {
            return PerformAction(ActionCut);
        }

        /// <summary>
        /// 对控件执行折叠操作，并返回是否操作成功。
        /// </summary>
        /// <returns></returns>
        public bool Collapse()
        {
            return PerformAction(ActionCollapse);
        }

        /// <summary>
        /// 对控件执行操作，并返回是否操作成功。
        /// </summary>
        /// <returns></returns>
        public bool Expand()
        {
            return PerformAction(ActionExpand);
        }

        /// <summary>
        /// 节点是否可以被解除。
        /// </summary>
        public bool Dismiss()
        {
            return PerformAction(ActionDismiss);
        }

        /// <summary>
        /// 显示屏幕
        /// </summary>
        /// <returns></returns>
        public bool Show()
        {
            return PerformAction(AccessibilityActionCompat.ActionShowOnScreen.Id);
        }

        /// <summary>
        /// 向前滚动节点内容的操作。
        /// </summary>
        /// <returns></returns>
        public bool ScrollForward()
        {
            return PerformAction(ActionScrollForward);
        }

        /// <summary>
        /// 向后滚动节点内容的操作。
        /// </summary>
        /// <returns></returns>
        public bool ScrollBackward()
        {
            return PerformAction(ActionScrollBackward);
        }

        /// <summary>
        /// 向上滚动节点内容的操作。
        /// </summary>
        /// <returns></returns>
        public bool ScrollUp()
        {
            return PerformAction(AccessibilityActionCompat.ActionScrollUp.Id);
        }

        /// <summary>
        /// 向下滚动节点内容的操作。
        /// </summary>
        /// <returns></returns>
        public bool ScrollDown()
        {
            return PerformAction(AccessibilityActionCompat.ActionScrollDown.Id);
        }

        /// <summary>
        /// 向左滚动节点内容的操作。
        /// </summary>
        /// <returns></returns>
        public bool ScrollLeft()
        {
            return PerformAction(AccessibilityActionCompat.ActionScrollLeft.Id);
        }

        /// <summary>
        /// 向右滚动节点内容的操作。
        /// </summary>
        /// <returns></returns>
        public bool ScrollRight()
        {
            return PerformAction(AccessibilityActionCompat.ActionScrollRight.Id);
        }

        /// <summary>
        /// 对节点的内容点击操作。
        /// </summary>
        /// <returns></returns>
        public bool ContextClick()
        {
            return PerformAction(AccessibilityActionCompat.ActionContextClick.Id);
        }

        /// <summary>
        /// 设置选中
        /// </summary>
        /// <param name="start">选中的起始位置</param>
        /// <param name="end">选中的结束位置</param>
        /// <returns></returns>
        public bool SetSelection(int start, int end)
        {
            var bundle = new Bundle();
            bundle.PutInt(ActionArgumentSelectionStartInt, start);
            bundle.PutInt(ActionArgumentSelectionEndInt, end);

            return PerformAction(ActionSetSelection, bundle);
        }

        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public bool SetText(string text)
        {
            var bundle = new Bundle();
            bundle.PutString(ActionArgumentSetTextCharsequence, text);
            return PerformAction(ActionSetText, bundle);
        }

        /// <summary>
        /// 设置进度
        /// </summary>
        /// <param name="value">进度</param>
        /// <returns></returns>
        public bool SetProgress(float value)
        {
            var bundle = new Bundle();
            bundle.PutFloat(ActionArgumentProgressValue, value);
            return PerformAction(AccessibilityActionCompat.ActionSetProgress.Id, bundle);
        }

        /// <summary>
        /// 滚动到
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="column">列</param>
        /// <returns></returns>
        public bool ScrollTo(int row, int column)
        {
            var bundle = new Bundle();
            bundle.PutInt(ActionArgumentRowInt, row);
            bundle.PutInt(ActionArgumentColumnInt, column);
            return PerformAction(AccessibilityActionCompat.ActionScrollToPosition.Id, bundle);
        }

        #endregion 控件基础操作

        #region 控件查找操作

        /// <inheritdoc cref="Bounds" />
        /// <summary>
        /// 返回控件在屏幕上的范围，其值是一个Rect对象。
        /// </summary>
        /// <returns></returns>
        public Rect Bounds()
        {
            var rect = new Rect();
            GetBoundsInScreen(rect);
            return rect;
        }

        /// <inheritdoc cref="BoundsInParent" />
        /// <summary>
        /// 返回控件在父控件中的范围，其值是一个Rect对象。
        /// </summary>
        /// <returns></returns>
        public Rect BoundsInParent()
        {
            var rect = new Rect();
            GetBoundsInParent(rect);
            return rect;
        }

        /// <inheritdoc cref="Child" />
        /// <summary>
        /// 获得节点的子集
        /// </summary>
        /// <param name="index">下标</param>
        /// <returns></returns>
        public UiNode Child(int index)
        {
            var child = GetChild(index);

            if (child == null)
            {
                return null;
            }

            var chil = child.Unwrap();

            return chil == null ? null : new UiNode(chil, Depth + 1, index);
        }

        /// <inheritdoc cref="Parent" />
        /// <summary>
        /// 获得节点的父级。
        /// </summary>
        /// <returns></returns>
        public new UiNode Parent()
        {
            try
            {
                var parent = base.Parent;

                if (parent == null)
                {
                    return null;
                }

                var paren = parent.Unwrap();

                return paren == null ? null : new UiNode(paren, Depth - 1);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region 重载执行参数

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="action">动作</param>
        /// <returns></returns>
        private new bool PerformAction(int action)
        {
            var bundle = new Bundle(action);
            try
            {
                return base.PerformAction(action, bundle);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        #endregion 重载执行参数
    }
}