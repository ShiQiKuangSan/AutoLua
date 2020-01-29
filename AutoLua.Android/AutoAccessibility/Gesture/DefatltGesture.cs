using System.Threading.Tasks;
using Android.AccessibilityServices;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AutoLua.Droid.Utils.Random;

namespace AutoLua.Droid.AutoAccessibility.Gesture
{
    [Preserve(AllMembers = true)]
    public class DefatltGesture : IGesture
    {
        /// <inheritdoc />
        /// <summary>
        /// 点击坐标
        /// </summary>
        /// <param name="x">要点击的坐标的x值</param>
        /// <param name="y">要点击的坐标的y值</param>
        /// <returns>是否点击成功</returns>
        public bool Click(int x, int y)
        {
            return Press(x, y, ViewConfiguration.TapTimeout + 50);
        }

        /// <inheritdoc />
        /// <summary>
        /// 长按坐标
        /// </summary>
        /// <param name="x">要长按的坐标的x值</param>
        /// <param name="y">要长按的坐标的y值</param>
        /// <returns>是否成功</returns>
        public bool LongClick(int x, int y)
        {
            return Gesture(ViewConfiguration.LongPressTimeout + 200, 0, null, new[] { x, y });
        }

        /// <inheritdoc />
        /// <summary>
        /// 按住坐标
        /// </summary>
        /// <param name="x">要按住的坐标的x值</param>
        /// <param name="y">要按住的坐标的y值</param>
        /// <param name="duration">按住时长，单位毫秒</param>
        /// <returns></returns>
        public bool Press(int x, int y, int duration)
        {
            return Gesture(duration, 0, null, new[] { x, y });
        }

        /// <inheritdoc />
        /// <summary>
        /// 从坐标(x1, y1)滑动到坐标(x2, y2)
        /// </summary>
        /// <param name="x1">滑动的起始坐标的x值</param>
        /// <param name="y1">滑动的起始坐标的y值</param>
        /// <param name="x2">滑动的结束坐标的x值</param>
        /// <param name="y2">滑动的结束坐标的y值</param>
        /// <param name="duration">滑动时长，单位毫秒</param>
        /// <returns>是否成功</returns>
        public bool Swipe(int x1, int y1, int x2, int y2, int duration)
        {
            return Gesture(duration, 0, null, new[] { x1, y1 }, new[] { x2, y2 });
        }

        /// <summary>
        /// 从坐标(x1, y1)滑动到坐标(x2, y2),滑动的路径是随机生成的
        /// </summary>
        /// <param name="x1">滑动的起始坐标的x值</param>
        /// <param name="y1">滑动的起始坐标的y值</param>
        /// <param name="x2">滑动的结束坐标的x值</param>
        /// <param name="y2">滑动的结束坐标的y值</param>
        /// <param name="duration">滑动时长，单位毫秒</param>
        /// <returns>是否成功</returns>
        public bool SwipeRandom(int x1, int y1, int x2, int y2, int duration)
        {
            var patch = new Path();
            patch.MoveTo(x1, y1);

            var n = (y1 - y2) / 10;

            for (var i = 0; i < 10; i++)
            {
                var sx = RandomManager.CreateRandomNumber(1, 30);
                var sy = RandomManager.CreateRandomNumber(1, 30);

                y1 = y1 - sy - n;

                if (y1 > 200)
                {
                    patch.LineTo(x1 + sx, y1);
                }
                else
                {
                    patch.LineTo(x2, y2);
                }
            }

            return Gestures(null, new GestureDescription.StrokeDescription(patch, 0, duration));
        }

        /// <inheritdoc />
        /// <summary>
        /// 手势操作
        /// </summary>
        /// <param name="duration">手势的时长</param>
        /// <param name="start">起始时间</param>
        /// <param name="handler">回调的处理函数</param>
        /// <param name="points">手势滑动路径的一系列坐标</param>
        /// <returns>是否成功</returns>
        public bool Gesture(int duration, long start = 0, Handler handler = null, params int[][] points)
        {
            var path = PointsToPath(points);
            return Gestures(handler, new GestureDescription.StrokeDescription(path, start, duration));
        }

        /// <inheritdoc />
        /// <summary>
        /// 同时模拟多个手势。每个手势的参数为[delay, duration, 坐标], delay为延迟多久(毫秒)才执行该手势；duration为手势执行时长
        /// </summary>
        /// <param name="handler">回调的处理程序</param>
        /// <param name="strokes">手势</param>
        /// <returns>是否成功</returns>
        public bool Gestures(Handler handler = null, params GestureDescription.StrokeDescription[] strokes)
        {
            var service = AutoAccessibilityService.Instance;

            if (service == null)
            {
                return false;
            }

            var builder = new GestureDescription.Builder();

            foreach (var stroke in strokes)
            {
                builder.AddStroke(stroke);
            }

            return handler == null
                ? GesturesWithoutHandler(builder.Build())
                : GesturesWithHandler(builder.Build(), handler);
        }

        /// <summary>
        /// 有处理程序的手势
        /// </summary>
        /// <param name="description">手势</param>
        /// <param name="handler">处理程序</param>
        /// <returns></returns>
        private static bool GesturesWithHandler(GestureDescription description, Handler handler)
        {
            //获得当前服务
            var service = AutoAccessibilityService.Instance;

            if (service == null)
                return false;

            var result = new CallBackData();

            var task = new Task(() =>
            {
                while (!result.IsEnd)
                {
                    System.Threading.Thread.Sleep(100);
                }
            });

            //回调函数
            var callBack = new WithCallBack(result);

            service.DispatchGesture(description, callBack, handler);

            task.Start();
            task.Wait();

            return result.Status;
        }

        /// <summary>
        /// 没有处理程序的手势
        /// </summary>
        /// <param name="description">手势</param>
        private static bool GesturesWithoutHandler(GestureDescription description)
        {
            //获得当前服务
            var service = AutoAccessibilityService.Instance;

            if (service == null)
                return false;

            var result = new CallBackData();

            var task = new Task(() =>
            {
                while (!result.IsEnd)
                {
                    System.Threading.Thread.Sleep(100);
                }
            });

            //回调函数
            var callBack = new WithCallBack(result);

            service.DispatchGesture(description, callBack, null);

            task.Start();
            task.Wait();

            return result.Status;
        }

        /// <summary>
        /// 获得手势路径。
        /// </summary>
        /// <param name="points">路径集合</param>
        /// <returns></returns>
        private Path PointsToPath(params int[][] points)
        {
            var path = new Path();
            //X的 横纵坐标
            var x = points[0][0];
            //Y的 横纵坐标
            var y = points[0][1];
            path.MoveTo(x, y);

            for (var i = 1; i < points.Length; i++)
            {
                var point = points[i];

                path.LineTo(point[0], point[1]);
            }

            return path;
        }


        /// <summary>
        /// 有手势的回调函数
        /// </summary>
        private class WithCallBack : AccessibilityService.GestureResultCallback
        {
            private readonly CallBackData _result;

            /// <summary>
            /// 有手势的回调函数
            /// </summary>
            public WithCallBack(CallBackData result)
            {
                _result = result;
            }

            public override void OnCompleted(GestureDescription gestureDescription)
            {
                _result.Status = true;
                _result.IsEnd = true;
            }

            public override void OnCancelled(GestureDescription gestureDescription)
            {
                _result.Status = false;
                _result.IsEnd = true;
            }
        }

        private class CallBackData
        {
            /// <summary>
            /// 是否成功
            /// </summary>
            public bool Status { get; set; } = false;

            /// <summary>
            /// 是否结束
            /// </summary>
            public bool IsEnd { get; set; } = false;
        }
    }
}