using System;
using System.Collections.Generic;
using Android.Runtime;
using AutoLua.Core.AutoAccessibility.Gesture;
using AutoLua.Core.Extensions;
using AutoLua.Core.LuaScript.Api;
using HttpServer.Modules;

namespace AutoLua.Droid.HttpServers.Controllers
{
    /// <summary>
    /// 手势控制器
    /// </summary>
    [Preserve(AllMembers = true)]
    public class GestureController : Controller
    {
        private readonly IGesture gesture;
        private readonly Device device;

        public GestureController()
        {
            gesture = new DefatltGesture();
            device = new Device();
        }

        /// <summary>
        /// 三阶赛贝尔曲线
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/gesture/quadTo")]
        public ActionResult QuadTo(int x1, int y1, int x2, int y2, int x3, int y3, int duration)
        {
            if (x1 <= 0 || y1 <= 0 || x2 <= 0 || y2 <= 0 || x3 <= 0 || y3 <= 0)
            {
                return JsonError("参数错误");
            }
            try
            {
                var status = gesture.Gesture(duration, x1, y1, x2, y2, x3, y3);

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 根据设定的系数进行滑动
        /// </summary>
        /// <param name="xishu">系数</param>
        /// <param name="type">滑动方向，“up” ，“down”</param>
        /// <param name="duration">滑动时间</param>
        /// <param name="swipePostionType">滑动位置，right：0 ， left：1</param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/gesture/swipeToXishu")]
        public ActionResult SwipeToXishu(double xishu, string type, int duration, int swipePostionType)
        {
            var x = device.width / 2;
            var y = device.height / 2;
            var zy = y - (y / 2);
            double a = 0;
            
            var random = new Random();

            if (type.ToLower() == "up")
            {
                var r = random.StrictNext(-50, 50);

                y -= zy;
                y += r;

                var xs = (device.height - y) / xishu;

                a = xs * 1.2;
            }

            if (type.ToLower() == "down")
            {
                var r = random.StrictNext(-50, 50);

                y += zy;
                y += r;

                a = (y / xishu) * 1.2;
            }

            double y1 = y;

            if (type.ToLower() == "up")
            {
                y1 = y + a;
            }

            if (type.ToLower() == "down")
            {
                y1 = y - a;
            }

            if (y1 < 0)
            {
                y1 = 10;
            }

            try
            {
                var status = this.SwipeMove(x, y, x, y1.To<int>(), duration, swipePostionType);

                return JsonSuccess(new { Status = status });
            }
            catch (Exception e)
            {

                return JsonError(e.Message);
            }
        }


        /// <summary>
        /// 滑动
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="duration"></param>
        /// <param name="swipePostionType"></param>
        /// <param name="isquxian"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/gesture/swipeMove")]
        public ActionResult SwipeMove(int x1, int y1, int x2, int y2, int duration, int swipePostionType = 0, bool isquxian = true)
        {
            if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0)
            {
                return JsonError("SwipeMoveTo 传递的滑动坐标不能小于0");
            }
            try
            {
                var status = SwipeMoveTo(x1, y1, x2, y2, duration, swipePostionType, isquxian);
                return JsonSuccess(new { Status = status });

            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }


        private bool SwipeMoveTo(int x1, int y1, int x2, int y2, int duration, int swipePostionType = 0, bool isquxian = true)
        {
            var random = new Random();

            var r = random.StrictNext(1, 10);

            if (r <= 4 && isquxian)
            {
                //曲线运动
                return SwipeMoveToPostion(x1, y1, x2, y2, duration, swipePostionType);
            }

            //直线运动

            var step = device.height / 10;

            var move = new Func<int, int, int>((from, to) =>
             {
                 if (from > to)
                 {
                     return -1 * step;
                 }
                 return step;
             });

            var x = x1;
            var y = y1;

            var rx = random.StrictNext(-60, 60);
            var ry = random.StrictNext(-60, 60);

            x += rx;
            y += ry;

            var postion = new List<int[]>();

            while (Math.Abs(x - x2) >= step || Math.Abs(y - y2) >= step)
            {
                var y_1 = y;
                if (Math.Abs(x - x2) >= step)
                {
                    x += move(x, x2);
                    y_1 += random.StrictNext(-30, 30);
                }

                var x_1 = x;

                if (Math.Abs(y - y2) >= step)
                {
                    y += move(y, y2);
                    x_1 += random.StrictNext(-30, 30);
                }

                postion.Add(new[] { x_1, y_1 });
            }

            var rx2 = random.StrictNext(-60, 60);
            var ry2 = random.StrictNext(-60, 60);

            var x_2 = (x2 + rx2);
            var y_2 = (y2 + ry2);

            postion.Add(new[] { x_2, y_2 });

            return gesture.Gesture(duration, 0, null, postion.ToArray());
        }

        private bool SwipeMoveToPostion(int x1, int y1, int x2, int y2, int duration, int swipePostionType = 0)
        {
            var getStartPostion = new Func<int, int, int[], int[]>((x, y, qujian) =>
            {
                var random = new Random();

                var rx = random.StrictNext(qujian[0], qujian[1]);
                var ry = random.StrictNext(qujian[0], qujian[1]);

                var startX = x + rx;
                var startY = y + ry;

                if (startX <= 0)
                {
                    startX = 10;
                }

                if (startX >= device.width)
                {
                    startX = device.width - 10;
                }

                if (startY <= 0)
                {
                    startY = 10;
                }

                if (startY >= device.height)
                {
                    startY = device.height - 10;
                }

                return new[] { startX, startY };
            });

            var getCenterPostion = new Func<int, int, int, int[], int[]>((startY, x, y, qujian) =>
             {
                 var zx = x;
                 var zy = y;

                 var random = new Random();

                 var rx = random.StrictNext(qujian[0], qujian[1]);
                 var ry = random.StrictNext(qujian[0], qujian[1]);

                 var qj = Math.Abs(zy - startY);

                 if (qj < qujian[1])
                 {
                     rx = random.StrictNext(-qj, qj);
                     ry = random.StrictNext(-qj, qj);
                 }

                 zx += rx;
                 zy += ry;

                 if (zx <= 0)
                 {
                     zx = 0;
                 }

                 if (zx >= device.width)
                 {
                     zx = x;
                 }

                 if (zy <= 0)
                 {
                     zy = 0;
                 }

                 if (zy >= device.height)
                 {
                     zy = y;
                 }

                 return new[] { zx, zy };
             });

            var getEndPostion = new Func<int, int[], int, int[]>((y2, qujian, postionType) =>
            {
                var endX = device.width - 40;
                var endY = y2;

                if (postionType >= 1)
                {
                    endX = 40;
                }

                var random = new Random();

                var rx = random.StrictNext(qujian[0], qujian[1]);
                var ry = random.StrictNext(qujian[0], qujian[1]);

                endX += rx;

                if (endY <= 0)
                {
                    endY = 40;
                }

                if (endY >= device.width)
                {
                    endY = device.height - 40;
                }

                endY += ry;

                return new[] { endX, endY };
            });


            var postion = getStartPostion(x1, y1, new[] { -50, 50 });

            var startX = postion[0];
            var startY = postion[1];

            var p = new List<int[]>();

            p.Add(new[] { startX, startY });

            var centerXy = getCenterPostion(startX, x2, y2, new[] { -40, 40 });

            var centerX = centerXy[0];
            var centerY = centerXy[1];

            p.Add(new[] { centerX, centerY });

            var endXy = getEndPostion(y2, new[] { -30, 30 }, swipePostionType);
            var endX = endXy[0];
            var endY = endXy[1];

            p.Add(new[] { endX, endY });

            if (centerX < 0 || centerY < 0 || endX < 0 || endY < 0)
            {
                return false;
            }

            var arr = GetPostions(p);

            return gesture.Gesture(duration, 0, null, arr);

        }

        private static int[][] GetPostions(List<int[]> point, int n = 2)
        {
            var m = 0.05;

            var fx = new Func<double, double>((t) =>
            {
                return XFunc(point, 0, 0, t, n);
            });

            var fy = new Func<double, double>((t) =>
            {
                return YFunc(point, 0, 0, t, n);
            });

            var random = new Random();
            double t = 0;

            var postion = new List<int[]>();

            for (var i = 0; t <= 1; i++)
            {
                var xR = random.StrictNext(-5, 5);
                var yR = random.StrictNext(-5, 5);
                var x_ = fx(t) + xR;
                var y_ = fy(t) + yR;

                t += m;
                postion.Add(new[] { x_.ToInt32(), y_.ToInt32() });
            }

            return postion.ToArray();
        }

        private static int XFunc(List<int[]> point, int i, int j, double t, int n)
        {
            if (i == n)
            {
                return point[j][0];
            }

            return ((1 - t) * XFunc(point, i + 1, j, t, n)).ToInt32() + (t * XFunc(point, i + 1, j + 1, t, n)).ToInt32();
        }

        private static int YFunc(List<int[]> point, int i, int j, double t, int n)
        {
            if (i == n)
                return point[j][1];

            return ((1 - t) * YFunc(point, i + 1, j, t, n)).ToInt32() + (t * YFunc(point, i + 1, j + 1, t, n)).ToInt32();
        }
    }
}