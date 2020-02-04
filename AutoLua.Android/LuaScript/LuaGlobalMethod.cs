using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.AccessibilityServices;
using Android.Content;
using Android.Support.V4.View;
using Android.Views.Accessibility;
using AutoLua.Droid.AutoAccessibility;
using AutoLua.Droid.AutoAccessibility.Gesture;
using AutoLua.Droid.Utils;
using AutoLua.Droid.Utils.Clip;
using AutoLua.Droid.Utils.Random;
using AutoLua.Events;
using Newtonsoft.Json;
using NLua;
using Xamarin.Forms;

namespace AutoLua.Droid.LuaScript
{
    [Android.Runtime.Preserve(AllMembers = true)]
    internal class LuaGlobalMethod
    {
        private readonly IGesture _gesture;

        internal LuaGlobalMethod(Context context)
        {
            _gesture = new DefatltGesture();
            ClipboardUtil.Context = context;
        }

        /// <summary>
        /// 脚本暂停
        /// </summary>
        /// <param name="time"></param>
        public void Sleep(int time)
        {
            Thread.Sleep(time);
        }

        /// <summary>
        /// 当前应用的包名
        /// </summary>
        public string CurrentPackage()
        {
            return AutoGlobal.Instance.AccessibilityEvent?.LatestPackage ?? string.Empty;
        }

        /// <summary>
        /// 返回当前应用的活动名
        /// </summary>
        /// <returns></returns>
        public string CurrentActivity()
        {
            return AutoGlobal.Instance.AccessibilityEvent?.LatestActivity ?? string.Empty;
        }

        /// <summary>
        /// 设置剪切板
        /// </summary>
        /// <param name="text"></param>
        public void SetClip(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            ClipboardUtil.SetClip(text);
        }

        /// <summary>
        /// 获得剪切板
        /// </summary>
        /// <returns></returns>
        public string GetClip()
        {
            return ClipboardUtil.GetClip();
        }

        /// <summary>
        /// toast
        /// </summary>
        /// <param name="message"></param>
        public void Toast(string message)
        {
            ToastUtils.ShowToast(message);
        }

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public void Print(object message)
        {
            switch (message)
            {
                case null:
                    return;
                case LuaTable table:
                {
                    var str = message.ToString();
                    if (str == "table")
                    {
                        Task.Run(() =>
                        {
                            var builder = new System.Text.StringBuilder();

                            PrintTable(table, builder, "\t");

                            PLog(builder.ToString());
                        });
                    }
                    
                    break;
                }
                default:
                    PLog(message.ToString());
                    break;
            }
        }


        private void PLog(string message)
        {
            LogEventDelegates.Instance.OnLog(new LogEventArgs("正常", message, Xamarin.Forms.Color.Blue));
            Sleep(200);
        }

        private static StringBuilder PrintTable(LuaTable table, StringBuilder builder, string t)
        {
            builder.AppendLine();

            foreach (var (k, v) in table.Items)
            {
                var tType = v.GetType();

                if (tType == typeof(LuaTable))
                {
                    builder.Append($"{t}[{k}] : {{");
                    var t1 = t + "\t";
                    PrintTable((LuaTable) v, builder, t1);

                    builder.AppendLine($"{t}}}");
                }
                else if (tType == typeof(LuaFunction))
                {
                    builder.AppendLine($"{t}[{k}] : function");
                }
                else if (tType == typeof(string))
                {
                    builder.AppendLine($"{t}[{k}] : '{v}'");
                }
                else
                {
                    builder.AppendLine($"{t}[{k}] : {v}");
                }
            }

            return builder;
        }

        /// <summary>
        /// 相当于toast(message);log(message)。显示信息message并在控制台中输出。参见console.log。
        /// </summary>
        /// <param name="message"></param>
        public void ToastLog(string message)
        {
            ToastUtils.ShowToast(message);
            PLog(message);
        }

        /// <summary>
        /// 等待指定的Activity出现，period为检查Activity的间隔。
        /// </summary>
        /// <param name="activity">Activity名称</param>
        /// <param name="period">轮询等待间隔（毫秒）</param>
        public void WaitForActivity(string activity, int period = 200)
        {
            while (CurrentActivity() != activity)
            {
                Sleep(period);
            }
        }

        /// <summary>
        /// 等待指定的应用出现。例如waitForPackage("com.tencent.mm")为等待当前界面为微信。
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="period">轮询等待间隔（毫秒）</param>
        public void WaitForPackage(string package, int period = 200)
        {
            while (CurrentPackage() != package)
            {
                Sleep(period);
            }
        }

        /// <summary>
        /// 停止脚本
        /// </summary>
        public void Exit()
        {
            try
            {
                AppApplication.Lua?.Close();
                LogEventDelegates.Instance.OnLog(new LogEventArgs("正常", "脚本结束运行", Xamarin.Forms.Color.Red));
            }
            catch (System.Exception e)
            {
                LogEventDelegates.Instance.OnLog(new LogEventArgs("异常", e.Message, Xamarin.Forms.Color.Red));
            }
        }

        /// <summary>
        /// 返回一个在[min...max]之间的随机数。例如random(0, 2)可能产生0, 1, 2。
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public double Random(double min, double max)
        {
            return RandomManager.CreateRandomNumberLowMin(min, max);
        }

        /// <summary>
        /// 返回在[0, 1)的随机浮点数。
        /// </summary>
        /// <returns></returns>
        public double Random()
        {
            return RandomManager.CreateRandomNumberLowMin(0, 1, true);
        }

        /// <summary>
        /// 点击坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Click(int x, int y)
        {
            return _gesture.Click(x, y);
        }

        /// <summary>
        /// 点击坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool LongClick(int x, int y)
        {
            return _gesture.LongClick(x, y);
        }

        /// <summary>
        /// 按住时间
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool Press(int x, int y, int duration)
        {
            return _gesture.Press(x, y, duration);
        }

        /// <summary>
        /// 滑动
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool Swipe(int x1, int y1, int x2, int y2, int duration)
        {
            return _gesture.Swipe(x1, y1, x2, y2, duration);
        }

        /// <summary>
        /// 手势，坐标集合
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool Gesture(int duration, params int[][] points)
        {
            return _gesture.Gesture(duration, 0, null, points);
        }

        /// <summary>
        /// 手势
        /// </summary>
        /// <param name="strokes"></param>
        /// <returns></returns>
        public bool Gestures(params GestureDescription.StrokeDescription[] strokes)
        {
            return _gesture.Gestures(null, strokes);
        }
    }
}