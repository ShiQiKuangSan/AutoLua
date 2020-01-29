using System;
using System.Collections.Generic;
using AutoLua.Droid.AutoAccessibility.Accessibility.Node;

namespace AutoLua.Droid.AutoAccessibility.Accessibility.Filter
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class UiNodeSearch : IUiNodeSearch
    {
        public IList<UiNode> Search(UiNode root, IExpressionExecutor filter, int max = int.MaxValue)
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

        private static List<UiNode> SearchChild(UiNode root, IExpressionExecutor filter, int max)
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

                    stack.Enqueue(child);
                }

                if (filter.Execute(parent))
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
    }
}