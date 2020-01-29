using AutoLua.Droid.AutoAccessibility.Accessibility.Node;
using System.Collections.Generic;

namespace AutoLua.Droid.AutoAccessibility.Accessibility.Filter
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public interface IUiNodeSearch
    {
        IList<UiNode> Search(UiNode root, IExpressionExecutor filter, int max = int.MaxValue);
    }
}