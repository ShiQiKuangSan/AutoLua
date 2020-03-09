using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Views;
using AutoLua.Core.Common;
using AutoLua.Core.LuaScript.ApiCommon.MaterialDialogs;
using NLua;
using NLua.Exceptions;
using static AFollestad.MaterialDialogs.MaterialDialog;

namespace AutoLua.Core.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class Dialogs
    {
        private ContextThemeWrapper _themeWrapper;

        public void SetContextTheme(ContextThemeWrapper contextThemeWrapper)
        {
            _themeWrapper = contextThemeWrapper;
        }
        
        /// <summary>
        /// 显示一个只包含“确定”按钮的提示对话框。直至用户点击确定脚本才继续运行。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="content">可选，对话框的内容。默认为空。</param>
        /// <param name="callback">回调函数，可选。当用户点击确定时被调用</param>
        public void alert(string title, string content = "", Action callback = null)
        {
            var dialog = (BlockedMaterialDialog.Builder) DialogBuilder()
                .Alert(callback)
                .Title(title)
                .Content(content)
                .PositiveText("确定");

            dialog.ShowAndGet<object>();
        }

        /// <summary>
        /// 显示一个包含“确定”和“取消”按钮的提示对话框。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="content">可选，对话框的内容。默认为空。</param>
        /// <param name="callback">如果用户点击“确定”则返回 true ，否则返回 false</param>
        public bool confirm(string title, string content = "", Action<bool> callback = null)
        {
            var dialog = (BlockedMaterialDialog.Builder) DialogBuilder()
                .Confirm(callback)
                .Title(title)
                .Content(content)
                .PositiveText("确定")
                .NegativeText("取消");

            return dialog.ShowAndGet<bool>();
        }

        /// <summary>
        /// 显示一个包含输入框的对话框，等待用户输入内容，并在用户点击确定时将输入的字符串返回。如果用户取消了输入，返回null。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="prefill">输入框的初始内容，可选，默认为空。</param>
        /// <param name="callback">回调函数，可选。当用户点击确定时被调用。</param>
        public string rawInput(string title, string prefill = "", Action<string> callback = null)
        {
            var dialog = (BlockedMaterialDialog.Builder) DialogBuilder()
                .Title(title);

            dialog.Inputs(null, prefill, true, callback);
            return dialog.ShowAndGet<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="callback"></param>
        public string input(string title, string content = "", Action<string> callback = null)
        {
            return rawInput(title, content, callback);
        }

        /// <summary>
        /// 显示一个带有选项列表的对话框，等待用户选择，返回用户选择的文字。如果用户取消了选择，返回空。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="items">对话框的选项列表，是一个字符串数组。</param>
        /// <param name="callback">回调函数，当用户点击确定时被调用</param>
        public string select(string title, LuaTable items, Action<string> callback = null)
        {
            var dialog = (BlockedMaterialDialog.Builder) DialogBuilder()
                .ItemsCallback(callback)
                .Title(title);

            var list = new List<string>();

            if (items != null)
            {
                list = items.Items.Values.Select(x => x.ToString()).ToList();
            }

            dialog.Items(list);
            return dialog.ShowAndGet<string>();
        }

        /// <summary>
        /// 显示一个单选列表对话框，等待用户选择，返回用户选择的选项索引(0 ~ item.length - 1)。如果用户取消了选择，返回-1。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="selectedIndex">对话框的初始选项的位置，默认为0。</param>
        /// <param name="items">对话框的选项列表，是一个字符串数组。</param>
        /// <param name="callback">回调函数。当用户点击确定时被调用。</param>
        public string singleChoice(string title, int selectedIndex, LuaTable items, Action<int, string> callback = null)
        {
            var dialog = (BlockedMaterialDialog.Builder) DialogBuilder()
                .ItemsCallbackSingleChoice(selectedIndex, callback)
                .Title(title)
                .PositiveText("确定");

            var list = new List<string>();

            if (items != null)
            {
                list = items.Items.Values.Select(x => x.ToString()).ToList();
            }

            dialog.Items(list);
            return dialog.ShowAndGet<string>();
        }

        /// <summary>
        /// 显示一个多选列表对话框，等待用户选择，返回用户选择的选项索引的数组。如果用户取消了选择，返回[]。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="indices">选项列表中初始选中的项目索引的数组，默认为空数组。</param>
        /// <param name="items">对话框的选项列表，是一个字符串数组。</param>
        /// <param name="callback">回调函数，可选。当用户点击确定时被调用。</param>
        public int[] multiChoice(string title, LuaTable indices, LuaTable items, Action<int[], string[]> callback = null)
        {
            var selectedIndexs=new List<int>();
            
            if (indices != null)
            {
                selectedIndexs = items.Items.Values.Select(x =>
                {
                    if (int.TryParse(x.ToString(),out var res))
                    {
                        return res;
                    }
                    
                    return -1;
                } ).Where(x=>  x >= 0).ToList();
            }
            
            var dialog = (BlockedMaterialDialog.Builder) DialogBuilder()
                .ItemsCallbackMultiChoice(selectedIndexs.ToArray(), callback)
                .Title(title)
                .PositiveText("确定");

            var list = new List<string>();

            if (items != null)
            {
                list = items.Items.Values.Select(x => x.ToString()).ToList();
            }

            dialog.Items(list);
            return dialog.ShowAndGet<int[]>();
        }

        public Builder newBuilder()
        {
            var context = GetActivityContext();
            var builder = new Builder(context)
                .Theme(Theme.Light);
            return builder;
        }

        private Context GetActivityContext()
        {
            Context context = AppUtils.CurrentActivity;

            if (context == null && (context as Activity).IsFinishing)
            {
                context = GetContext();
            }

            return context;
        }

        private Context GetContext()
        {
            if (_themeWrapper != null)
                return _themeWrapper;

            throw new LuaException("Dialogs ContextThemeWrapper  is null");
        }

        private BlockedMaterialDialog.Builder DialogBuilder()
        {
            var context = GetActivityContext();
            return (BlockedMaterialDialog.Builder) new BlockedMaterialDialog.Builder(context)
                .Theme(Theme.Light);
        }
    }
}