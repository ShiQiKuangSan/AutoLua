using AutoLua.Droid.AutoAccessibility.Accessibility.Node;

namespace AutoLua.Droid.AutoAccessibility.Accessibility.Filter
{
    /// <summary>
    /// 表达式执行者。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public interface IExpressionExecutor
    {
        /// <summary>
        /// 执行。
        /// </summary>
        /// <returns>结果。</returns>
        bool Execute(UiNode source);
    }
}