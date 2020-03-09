using System;
using AutoLua.Core.AutoAccessibility.Accessibility.Node;

namespace AutoLua.Core.AutoAccessibility.Accessibility.Filter
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ExpressionExecutor : IExpressionExecutor
    {
        private readonly Func<UiNode, bool> _predicate;

        #region 构造器

        public ExpressionExecutor(Func<UiNode, bool> predicate)
        {
            _predicate = predicate;
        }

        #endregion 构造器

        /// <inheritdoc />
        /// <summary>
        /// 执行。
        /// </summary>
        /// <returns>结果。</returns>
        public bool Execute(UiNode source)
        {
            var status = _predicate(source);

            return status;
        }
    }
}