using Android.AccessibilityServices;

using AutoLua.Droid.AutoAccessibility;

namespace AutoLua.Droid.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class KeysApi
    {
        /// <summary>
        /// 模拟按下返回键。返回是否执行成功。
        /// </summary>
        public bool back()
        {
            return PerformGlobalAction(GlobalAction.Back);
        }

        /// <summary>
        /// 模拟按下Home键。返回是否执行成功。
        /// </summary>
        /// <returns></returns>
        public bool home()
        {
            return PerformGlobalAction(GlobalAction.Home);
        }

        /// <summary>
        /// 弹出电源键菜单。返回是否执行成功。 
        /// </summary>
        /// <returns></returns>
        public bool powerDialog()
        {
            return PerformGlobalAction(GlobalAction.PowerDialog);
        }

        /// <summary>
        /// 拉出通知栏。返回是否执行成功。
        /// </summary>
        /// <returns></returns>
        public bool notifications()
        {
            return PerformGlobalAction(GlobalAction.Notifications);
        }

        /// <summary>
        /// 显示快速设置(下拉通知栏到底)。返回是否执行成功。
        /// </summary>
        /// <returns></returns>
        public bool quickSettings()
        {
            return PerformGlobalAction(GlobalAction.QuickSettings);
        }

        /// <summary>
        /// 显示最近任务。返回是否执行成功。 
        /// </summary>
        /// <returns></returns>
        public bool recents()
        {
            return PerformGlobalAction(GlobalAction.Recents);
        }

        private static bool PerformGlobalAction(GlobalAction action)
        {
            AutoGlobal.Instance.EnsureAccessibilityServiceEnabled();

            if (AutoAccessibilityService.Instance == null)
                return false;

            var service = AutoAccessibilityService.Instance;

            return service.PerformGlobalAction(action);
        }
    }
}