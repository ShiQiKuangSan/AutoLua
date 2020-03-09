using System.Collections.Generic;
using AutoLua.Core.AutoAccessibility.Accessibility.Node;

namespace AutoLua.Core.AutoAccessibility.Accessibility.Filter
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public interface IUiNodeSearch
    {
        IEnumerable<UiNode> Search(UiNode root, IExpressionExecutor filter, int max = int.MaxValue);
    }
}