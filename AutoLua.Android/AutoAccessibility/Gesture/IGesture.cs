using Android.AccessibilityServices;
using Android.OS;
using Android.Runtime;

namespace AutoLua.Droid.AutoAccessibility.Gesture
{
    /// <summary>
    /// 手势接口，之后可能会实现一个root操作的手势
    /// </summary>
    [Preserve(AllMembers = true)]
    public interface IGesture
    {
        /// <summary>
        /// 点击坐标
        /// </summary>
        /// <param name="x">要点击的坐标的x值</param>
        /// <param name="y">要点击的坐标的y值</param>
        /// <returns>是否点击成功</returns>
        bool Click(int x, int y);

        /// <summary>
        /// 长按坐标
        /// </summary>
        /// <param name="x">要长按的坐标的x值</param>
        /// <param name="y">要长按的坐标的y值</param>
        /// <returns>是否成功</returns>
        bool LongClick(int x, int y);

        /// <summary>
        /// 按住坐标
        /// </summary>
        /// <param name="x">要按住的坐标的x值</param>
        /// <param name="y">要按住的坐标的y值</param>
        /// <param name="duration">按住时长，单位毫秒</param>
        /// <returns></returns>
        bool Press(int x, int y, int duration);

        /// <summary>
        /// 从坐标(x1, y1)滑动到坐标(x2, y2)
        /// </summary>
        /// <param name="x1">滑动的起始坐标的x值</param>
        /// <param name="y1">滑动的起始坐标的y值</param>
        /// <param name="x2">滑动的结束坐标的x值</param>
        /// <param name="y2">滑动的结束坐标的y值</param>
        /// <param name="duration">滑动时长，单位毫秒</param>
        /// <returns>是否成功</returns>
        bool Swipe(int x1, int y1, int x2, int y2, int duration);

        /// <summary>
        /// 从坐标(x1, y1)滑动到坐标(x2, y2),滑动的路径是随机生成的
        /// </summary>
        /// <param name="x1">滑动的起始坐标的x值</param>
        /// <param name="y1">滑动的起始坐标的y值</param>
        /// <param name="x2">滑动的结束坐标的x值</param>
        /// <param name="y2">滑动的结束坐标的y值</param>
        /// <param name="duration">滑动时长，单位毫秒</param>
        /// <returns>是否成功</returns>
        bool SwipeRandom(int x1, int y1, int x2, int y2, int duration);

        /// <summary>
        /// 手势操作
        /// </summary>
        /// <param name="duration">手势的时长</param>
        /// <param name="start">起始时间</param>
        /// <param name="handler">回调的处理函数</param>
        /// <param name="points">手势滑动路径的一系列坐标</param>
        /// <returns>是否成功</returns>
        bool Gesture(int duration, long start = 0, Handler handler = null, params int[][] points);

        /// <summary>
        /// 同时模拟多个手势。每个手势的参数为[delay, duration, 坐标], delay为延迟多久(毫秒)才执行该手势；duration为手势执行时长
        /// </summary>
        /// <param name="strokes">手势</param>
        /// <param name="handler">回调的处理程序</param>
        /// <returns>是否成功</returns>
        bool Gestures(Handler handler = null, params GestureDescription.StrokeDescription[] strokes);
    }
}