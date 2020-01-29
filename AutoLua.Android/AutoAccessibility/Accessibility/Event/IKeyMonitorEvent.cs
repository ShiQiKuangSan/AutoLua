using Android.Views;

namespace AutoLua.Droid.AutoAccessibility.Accessibility.Event
{
    /// <summary>
    /// 按键监听事件。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public interface IKeyMonitorEvent
    {
        /// <summary>
        /// 监听按键
        /// </summary>
        /// <param name="keyCode">按键码</param>
        /// <param name="event">按键事件</param>
        public void OnKeyEvent(Keycode keyCode, KeyEvent @event);
    }
}