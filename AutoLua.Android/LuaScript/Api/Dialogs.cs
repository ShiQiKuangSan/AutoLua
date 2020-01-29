using System;
using System.Collections.Generic;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AutoLua.Droid.Utils;
using Java.Lang;
using Newtonsoft.Json;
using NLua;
using static AFollestad.MaterialDialogs.MaterialDialog;

namespace AutoLua.Droid.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class Dialogs : Java.Lang.Object
    {
        private ContextThemeWrapper mThemeWrapper;

        /// <summary>
        /// 显示一个只包含“确定”按钮的提示对话框。直至用户点击确定脚本才继续运行。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="content">可选，对话框的内容。默认为空。</param>
        /// <param name="callback">回调函数，可选。当用户点击确定时被调用</param>
        public void alert(string title, string content = "", Action callback = null)
        {
            var context = GetActivityContext();

            var result = new CallBackData();

            var action = new Action(() =>
            {
                if (result.IsEnd)
                    return;

                result.IsEnd = true;
                callback?.Invoke();
            });

            var builder = new MaterialDialog.Builder(context)
                .Title(title)
                .PositiveText("确定")
                .DismissListener(dialog =>
                {
                    if (result.IsEnd)
                        return;

                    action.Invoke();
                })
                .OnAny((dialog, which) =>
                {
                    action.Invoke();
                });

            if (!string.IsNullOrWhiteSpace(content))
                builder.Content(content);

            Show(builder);
        }

        /// <summary>
        /// 显示一个包含“确定”和“取消”按钮的提示对话框。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="content">可选，对话框的内容。默认为空。</param>
        /// <param name="callback">如果用户点击“确定”则返回 true ，否则返回 false</param>
        public void confirm(string title, string content = "", Action<bool> callback = null)
        {
            var context = GetActivityContext();

            var result = new CallBackData();

            var action = new Action<bool>((value) =>
            {
                if (result.IsEnd)
                    return;

                result.IsEnd = true;
                callback?.Invoke(value);
            });

            var builder = new MaterialDialog.Builder(context)
                .Title(title)
                .PositiveText("确定")
                .NegativeText("取消")
                .DismissListener(dialog =>
                {
                    if (result.IsEnd)
                        return;

                    action.Invoke(false);
                })
                .OnAny((dialog, which) =>
                {
                    if (which == DialogAction.Positive)
                    {
                        action.Invoke(true);
                    }
                    else
                    {
                        action.Invoke(false);
                    }
                });

            if (!string.IsNullOrWhiteSpace(content))
                builder.Content(content);

            Show(builder);
        }

        /// <summary>
        /// 显示一个包含输入框的对话框，等待用户输入内容，并在用户点击确定时将输入的字符串返回。如果用户取消了输入，返回null。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="content">输入框的初始内容，可选，默认为空。</param>
        /// <param name="callback">回调函数，可选。当用户点击确定时被调用。</param>
        public void rawInput(string title, string content = "", Action<string> callback = null)
        {
            var context = GetActivityContext();

            var result = new CallBackData();

            var action = new Action<string>((value) =>
            {
                if (result.IsEnd)
                    return;

                result.IsEnd = true;
                callback?.Invoke(value);
            });

            var builder = new MaterialDialog.Builder(context)
              .Input(null, content, true, new InputCallback((input) => action(input)))
              .Title(title)
              .CancelListener(dialog =>
              {
                  action.Invoke(string.Empty);
              });

            Show(builder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="callback"></param>
        public void input(string title, string content = "", Action<string> callback = null)
        {
            rawInput(title, content, callback);
        }

        /// <summary>
        /// 显示一个带有选项列表的对话框，等待用户选择，返回用户选择的文字。如果用户取消了选择，返回空。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="items">对话框的选项列表，是一个字符串数组。</param>
        /// <param name="callback">回调函数，当用户点击确定时被调用</param>
        public void select(string title, LuaTable items, Action<string> callback = null)
        {
            var context = GetActivityContext();

            var result = new CallBackData();

            var action = new Action<string>((value) =>
            {
                if (result.IsEnd)
                    return;

                result.IsEnd = true;
                callback?.Invoke(value);
            });

            var list = new List<string>();

            try
            {
                if (items != null)
                {
                    var str = JsonConvert.SerializeObject(items);

                    var table = JsonConvert.DeserializeObject<TableLua>(str);

                    list = table.Values;
                }
            }
            catch (System.Exception)
            {
            }

            var builder = new MaterialDialog.Builder(context)
                .DismissListener((dialog) => action(string.Empty))
                .ItemsCallback(new Action<MaterialDialog, View, int, string>((dialog, itemView, position, text) => action(text)))
                .Title(title)
                .Items(list);

            Show(builder);
        }

        /// <summary>
        /// 显示一个单选列表对话框，等待用户选择，返回用户选择的选项索引(0 ~ item.length - 1)。如果用户取消了选择，返回-1。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="selectedIndex">对话框的初始选项的位置，默认为0。</param>
        /// <param name="items">对话框的选项列表，是一个字符串数组。</param>
        /// <param name="callback">回调函数。当用户点击确定时被调用。</param>
        public void singleChoice(string title, int selectedIndex, LuaTable items, Action<int, string> callback = null)
        {
            var context = GetActivityContext();

            var result = new CallBackData();

            var action = new Func<int, string, bool>((point, value) =>
            {
                if (result.IsEnd)
                    return false;

                result.IsEnd = true;
                callback?.Invoke(point, value);
                return true;
            });

            var list = new List<string>();

            try
            {
                if (items != null)
                {
                    var str = JsonConvert.SerializeObject(items);

                    var table = JsonConvert.DeserializeObject<TableLua>(str);

                    list = table.Values;
                }
            }
            catch (System.Exception)
            {
            }

            var builder = new MaterialDialog.Builder(context)
                .DismissListener((dialog) => action(0, string.Empty))
                .ItemsCallbackSingleChoice(selectedIndex, new Func<MaterialDialog, View, int, string, bool>((dialog, itemView, which, text) => action(which, text)))
                .Title(title)
                .PositiveText("确定")
                .Items(list);

            Show(builder);
        }

        /// <summary>
        /// 显示一个多选列表对话框，等待用户选择，返回用户选择的选项索引的数组。如果用户取消了选择，返回[]。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="indices">选项列表中初始选中的项目索引的数组，默认为空数组。</param>
        /// <param name="items">对话框的选项列表，是一个字符串数组。</param>
        /// <param name="callback">回调函数，可选。当用户点击确定时被调用。</param>
        public void multiChoice(string title, LuaTable indices, LuaTable items, Action<int[], string[]> callback = null)
        {
            var context = GetActivityContext();

            var result = new CallBackData();

            var action = new Func<int[], string[], bool>((point, value) =>
             {
                 if (result.IsEnd)
                     return false;

                 result.IsEnd = true;
                 callback?.Invoke(point, value);
                 return true;
             });

            var list = new List<string>();
            var tableIndicesList = new List<int>();
            try
            {
                if (indices != null)
                {
                    var strIndices = JsonConvert.SerializeObject(indices);
                    var tableIndices = JsonConvert.DeserializeObject<TableLua>(strIndices);

                    foreach (var x in tableIndices.Values)
                    {
                        var status = int.TryParse(x, out int res);

                        if (status)
                            tableIndicesList.Add(res);
                    }
                }
                if (items != null)
                {
                    var str = JsonConvert.SerializeObject(items);

                    var table = JsonConvert.DeserializeObject<TableLua>(str);

                    list = table.Values;
                }
            }
            catch (System.Exception)
            {
            }

            var builder = new MaterialDialog.Builder(context)
                .DismissListener((dialog) => action(new int[0], new string[0]))
                .ItemsCallbackMultiChoice(tableIndicesList.ToArray(), new Func<MaterialDialog, int[], string[], bool>((dialog, which, text) => action(which, text)))
                .Title(title)
                .PositiveText("确定")
                .Items(list);

            Show(builder);
        }

        public MaterialDialog.Builder newBuilder()
        {
            var context = GetActivityContext();
            var builder = new MaterialDialog.Builder(context)
                .Theme(Theme.Light);
            return builder;
        }

        /// <summary>
        /// 显示弹窗
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        private void Show(MaterialDialog.Builder builder)
        {
            if (Looper.MainLooper == Looper.MyLooper())
            {
                builder.Show();
            }
            else
            {
                AppUtils.RunOnUI(() =>
                {
                    builder.Show();
                });
            }
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
            if (mThemeWrapper != null)
                return mThemeWrapper;

            mThemeWrapper = new ContextThemeWrapper(AppUtils.GetAppContext.ApplicationContext, Resource.Style.Theme_AppCompat_Light);
            return mThemeWrapper;
        }


        private class CallBackData
        {
            /// <summary>
            /// 是否结束
            /// </summary>
            public bool IsEnd { get; set; } = false;
            public object Data { get; set; }
        }


        private class InputCallback : Java.Lang.Object, IInputCallback
        {
            private readonly Action<string> action;

            public InputCallback(Action<string> action)
            {
                this.action = action;
            }

            public void OnInput(MaterialDialog dialog, ICharSequence input)
            {
                action.Invoke(input.ToString());
            }
        }

        private class TableLua
        {
            public List<string> Keys { get; set; }

            public List<string> Values { get; set; }
        }
    }
}