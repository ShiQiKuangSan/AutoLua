using System;
using System.Linq;
using System.Collections.Generic;
using AutoLua.Core.AutoAccessibility.Accessibility.Node;

namespace AutoLua.Core.AutoAccessibility.Accessibility.Filter
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class UiNodeSearch : IUiNodeSearch
    {
        public IEnumerable<UiNode> Search(UiNode root, IEnumerable<IExpressionExecutor> filter, int max = int.MaxValue)
        {
            try
            {
                //添加任务
                return SearchChild(root, filter, max);
            }
            catch (Exception)
            {
                return new List<UiNode>();
            }
        }

        private static IEnumerable<UiNode> SearchChild(UiNode root, IEnumerable<IExpressionExecutor> filter, int max)
        {
            var result = new List<UiNode>();
            var stack = new Queue<UiNode>();
            stack.Enqueue(root);
            while (stack.Count > 0)
            {
                var parent = stack.Dequeue();

                for (var i = 0; i < parent.ChildCount; i++)
                {
                    var child = parent.Child(i);
                    if (child == null)
                    {
                        continue;
                    }

                    if (!child?.VisibleToUser ?? false)
                        continue;

                    stack.Enqueue(child);
                }

                if (Filter(parent, filter))
                {
                    result.Add(parent);

                    if (result.Count >= max)
                    {
                        break;
                    }
                }
                else
                {
                    if (parent != root)
                    {
                        parent.Recycle();
                    }
                }
            }

            return result;
        }


        private static bool Filter(UiNode uiNode, IEnumerable<IExpressionExecutor> filters)
        {
            foreach (var filter in filters)
            {
                if (!filter.Execute(uiNode))
                {
                    return false;
                }
            }

            return true;
        }
    }
}