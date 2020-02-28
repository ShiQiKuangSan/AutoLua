using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AutoLua.Droid.Expansions
{
    /// <summary>
    /// 枚举器扩展
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 判断当前数组是否与目标数组的开始元素一致，一致则返回开始索引。
        /// </summary>
        /// <typeparam name="TSource">数组类型。</typeparam>
        /// <param name="sources">当前数组。</param>
        /// <param name="targets">目标数组。</param>
        /// <returns>匹配返回索引，否则返回 -1。</returns>
        public static int IndexOfStarts<TSource>(this TSource[] sources, TSource[] targets) => sources.IndexOfStarts(0, targets, 0);

        /// <summary>
        /// 判断当前数组是否与目标数组的开始元素一致，一致则返回开始索引。
        /// </summary>
        /// <remarks>
        /// 1.sources = { 1, 2, 3 }
        ///   targets = { 2, 3, 4 }
        /// 当 index = 0 或者 1 或者 2 时。
        /// 当 targetIndex = 0 或者 1 时。
        /// 虽然 sources 没有办法完整匹配 targets，但从 index 索引开始，则完整匹配 targets 的 targetIndex 索引开始的所有元素，所以返回 True。
        /// 即：sources 从索引 1 开始匹配，则匹配 targets 的 2, 3。
        /// sources 从索引 2 开始匹配，则匹配 targets 的 3。
        /// 都返回 True。
        /// </remarks>
        /// <typeparam name="TSource">数组类型。</typeparam>
        /// <param name="sources">当前数组。</param>
        /// <param name="index">要搜索当前数组的开始索引。</param>
        /// <param name="targets">目标数组。</param>
        /// <param name="targetIndex">要开始匹配目标数组的索引。</param>
        /// <returns>匹配返回索引，否则返回 -1。</returns>
        public static int IndexOfStarts<TSource>(this TSource[] sources, int index, TSource[] targets, int targetIndex)
        {
            if (sources == null || sources.Length == 0)
            {
                return -1;
            }

            if (targets == null || targets.Length == 0)
            {
                return -1;
            }

            //搜索索引不可以小于 0。
            if (index < 0)
            {
                return -1;
            }

            if (targetIndex < 0)
            {
                return -1;
            }

            if (index >= sources.Length)
            {
                return -1;
            }

            if (targetIndex >= targets.Length)
            {
                return -1;
            }

            int loop = targetIndex;
            int startIndex = 0;
            bool isExist = false;

            for (; index < sources.Length && loop < targets.Length; index++)
            {
                if (sources[index].Equals(targets[loop]))
                {
                    if (loop == 0)
                    {
                        startIndex = index;
                    }

                    isExist = true;

                    loop++;

                    if (loop == sources.Length || loop == targets.Length)
                    {
                        break;
                    }
                }
                else
                {
                    if (isExist)
                    {
                        isExist = false;
                        break;
                    }
                }
            }

            return isExist ? startIndex : -1;
        }

        /// <summary>
        /// 获得当前数组是否包含指定的数组的指定位置的所有信息的开始索引。
        /// </summary>
        /// <typeparam name="TSource">数组类型。</typeparam>
        /// <param name="sources">当前数组。</param>
        /// <param name="targets">目标数组。</param>
        /// <returns>匹配返回索引，否则返回 -1。</returns>
        public static int IndexOf<TSource>(this TSource[] sources, TSource[] targets) => sources.IndexOf(0, sources.Length, targets, 0, targets.Length);

        /// <summary>
        /// 获得当前数组是否包含指定的数组的指定位置的所有信息的开始索引。
        /// </summary>
        /// <typeparam name="TSource">数组类型。</typeparam>
        /// <param name="sources">当前数组。</param>
        /// <param name="index">要搜索当前数组的开始索引。</param>
        /// <param name="targets">目标数组。</param>
        /// <param name="targetIndex">要开始匹配目标数组的索引。</param>
        /// <returns>匹配返回索引，否则返回 -1。</returns>
        public static int IndexOf<TSource>(this TSource[] sources, int index, TSource[] targets, int targetIndex) => sources.IndexOf(index, sources.Length, targets, targetIndex, targets.Length);

        /// <summary>
        /// 获得当前数组是否包含指定的数组的指定位置的所有信息的开始索引。
        /// </summary>
        /// <typeparam name="TSource">数组类型。</typeparam>
        /// <param name="sources">当前数组。</param>
        /// <param name="index">要搜索当前数组的开始索引。</param>
        /// <param name="length">要搜索当前数组的长度。</param>
        /// <param name="targets">目标数组。</param>
        /// <param name="targetIndex">要开始匹配目标数组的索引。</param>
        /// <param name="targetLength">要匹配目标数组的数据位数。</param>
        /// <returns>匹配返回索引，否则返回 -1。</returns>
        public static int IndexOf<TSource>(this TSource[] sources, int index, int length, TSource[] targets, int targetIndex, int targetLength)
        {
            if (sources == null || sources.Length == 0)
            {
                return -1;
            }

            if (targets == null || targets.Length == 0)
            {
                return -1;
            }

            //源数组长度小于目标数组长度，则必然不可能完全匹配。
            if (sources.Length < targets.Length)
            {
                return -1;
            }

            //搜索长度大于源数组长度，更新搜索长度等于源数组长度。
            if (length + index > sources.Length)
            {
                length = sources.Length - index;
            }

            //搜索长度小于目标数组长度，则必然不可能完全匹配。
            if (length < targets.Length)
            {
                return -1;
            }

            //搜索索引不可以小于 0。
            if (index < 0)
            {
                return -1;
            }

            if (targetIndex < 0)
            {
                return -1;
            }

            if (index >= sources.Length)
            {
                return -1;
            }

            if (targetIndex >= targets.Length)
            {
                return -1;
            }

            if (targetLength + targetIndex > targets.Length)
            {
                targetLength = targets.Length - targetIndex;
            }

            int loop = targetIndex;
            int startIndex = 0;
            bool isExist = false;

            for (int i = index; i < index + length; i++)
            {
                if (sources[i].Equals(targets[loop]))
                {
                    if (loop == 0)
                    {
                        startIndex = i;
                    }

                    isExist = true;

                    loop++;

                    if (loop == targetLength)
                    {
                        break;
                    }
                }
                else
                {
                    if (isExist)
                    {
                        isExist = false;
                        break;
                    }
                }
            }

            //该判断不成立时，表示匹配数据不完整。
            if (isExist && loop != targets.Length)
            {
                isExist = false;
            }

            return isExist ? startIndex : -1;
        }

        /// <summary>
        /// 判断当前数组是否包含指定的数组的所有信息。
        /// </summary>
        /// <typeparam name="TSource">数组类型。</typeparam>
        /// <param name="sources">当前数组。</param>
        /// <param name="index">要搜索当前数组的开始索引。</param>
        /// <param name="length">要搜索当前数组的长度。</param>
        /// <param name="targets">要匹配的数组。</param>
        /// <returns>匹配返回 True，否则返回 False。</returns>
        public static bool Any<TSource>(this TSource[] sources, int index, int length, TSource[] targets)
            => sources.Any(index, length, targets, 0, targets.Length);

        /// <summary>
        /// 判断当前数组是否包含指定的数组的指定位置开始的所有信息。
        /// </summary>
        /// <typeparam name="TSource">数组类型。</typeparam>
        /// <param name="sources">当前数组。</param>
        /// <param name="index">要搜索当前数组的开始索引。</param>
        /// <param name="length">要搜索当前数组的长度。</param>
        /// <param name="targets">要匹配的数组。</param>
        /// <param name="targetIndex">要开始匹配目标数组的索引。</param>
        /// <param name="targetLength">要匹配目标数组的数据位数。</param>
        /// <returns>匹配返回 True，否则返回 False。</returns>
        public static bool Any<TSource>(this TSource[] sources, int index, int length, TSource[] targets, int targetIndex, int targetLength)
            => sources.IndexOf(index, length, targets, targetIndex, targetLength) > -1;

        /// <summary>
        /// 确认此数组的开头是否与目标数组一致。
        /// </summary>
        /// <typeparam name="TSource">数组类型。</typeparam>
        /// <param name="sources">当前数组。</param>
        /// <param name="targets">要匹配的数组。</param>
        /// <returns>匹配返回 True，否则返回 False。</returns>
        public static bool StartsWith<TSource>(this TSource[] sources, TSource[] targets) => sources.IndexOf(targets) == 0;

        /// <summary>
        /// 确认此数组的开头是否与目标数组指定位置开始的项一致。
        /// </summary>
        /// <typeparam name="TSource">数组类型。</typeparam>
        /// <param name="sources">当前数组。</param>
        /// <param name="targets">要匹配的数组。</param>
        /// <param name="targetIndex">要开始匹配目标数组的索引。</param>
        /// <param name="targetLength">要匹配目标数组的数据位数。</param>
        /// <returns>匹配返回 True，否则返回 False。</returns>
        public static bool StartsWith<TSource>(this TSource[] sources, TSource[] targets, int targetIndex, int targetLength)
        {
            return sources.IndexOf(0, targets.Length, targets, targetIndex, targetLength) == 0;
        }

        /// <summary>
        /// 确认此数组的结尾是否与目标数组一致。
        /// </summary>
        /// <typeparam name="TSource">数组类型。</typeparam>
        /// <param name="sources">当前数组。</param>
        /// <param name="targets">要匹配的数组。</param>
        /// <returns>匹配返回 True，否则返回 False。</returns>
        public static bool EndsWith<TSource>(this TSource[] sources, TSource[] targets)
            => sources.Any(sources.Length - targets.Length, targets.Length, targets);

        /// <summary>
        /// 确认此数组的结尾是否与目标数组一致。
        /// </summary>
        /// <typeparam name="TSource">数组类型。</typeparam>
        /// <param name="sources">当前数组。</param>
        /// <param name="targets">要匹配的数组。</param>
        /// <param name="targetIndex">要开始匹配目标数组的索引。</param>
        /// <returns>匹配返回 True，否则返回 False。</returns>
        public static bool EndsWith<TSource>(this TSource[] sources, TSource[] targets, int targetIndex)
            => sources.Any(sources.Length - targets.Length + targetIndex, targets.Length, targets);

        /// <summary>
        /// 循环执行方法。
        /// </summary>
        /// <typeparam name="TSource">对象类型。</typeparam>
        /// <param name="enumerable">对象列表。</param>
        /// <param name="action">方法。</param>
        public static void ForEach<TSource>(this IEnumerable<TSource> enumerable, Action<TSource> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        /// <summary>
        /// 并行循环执行方法。
        /// </summary>
        /// <typeparam name="TSource">对象类型。</typeparam>
        /// <param name="enumerable">对象列表。</param>
        /// <param name="action">方法。</param>
        public static void ForEachParallel<TSource>(this IEnumerable<TSource> enumerable, Action<TSource> action) => Parallel.ForEach(enumerable, action);

        /// <summary>
        /// 机关函数应用False时：单个AND无效，多个AND无效；单个OR有效，多个OR有效；混应时写在OR后面的AND有效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>(this IEnumerable<T> enumerable) => f => false;

        /// <summary>
        /// 将集合转换成只读集合。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="enumerable">对象列表。</param>
        /// <returns>对象列表。</returns>
        public static IList<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable) => new ReadOnlyCollection<T>(enumerable.ToList());

        /// <summary>
        /// Or 条件拼接。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);

            return Expression.Lambda<Func<T, bool>>(Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// And 条件拼接。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);

            return Expression.Lambda<Func<T, bool>>(Expression.And(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// 通过使用相等比较器对值进行比较返回序列中的非重复元素。
        /// </summary>
        /// <remarks>
        /// 示例1：var query = items.DistinctBy(p => p.Id);
        /// 示例2：var query = items.DistinctBy(p => new { p.Id, p.Name });
        /// </remarks>
        /// <typeparam name="TSource">集合类型。</typeparam>
        /// <typeparam name="TKey">比较元素的类型。</typeparam>
        /// <param name="source">集合。</param>
        /// <param name="keySelector">相等比较器（即：选择通过哪个元素来比较）。</param>
        /// <returns>非重复元素的集合。</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();

            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        /// <summary>
        /// 通过使用相等比较器对值进行比较生成两个序列的差集。
        /// </summary>
        /// <remarks>
        /// 即：返回所有属于 source 集合且不属于 target 集合的元素构成的集合。
        /// 示例1：var query = items1.ExceptBy(items2, item => item.Id);
        /// 示例2：var query = items.ExceptBy(items2, p => new { p.Id, p.Name });
        /// </remarks>
        /// <typeparam name="TSource">集合类型。</typeparam>
        /// <typeparam name="TKey">比较元素的类型。</typeparam>
        /// <param name="source">源集合。</param>
        /// <param name="target">过虑集合。</param>
        /// <param name="keySelector">相等比较器（即：选择通过哪个元素来比较）。</param>
        /// <returns>source 集合与 target 集合的差集。</returns>
        public static IEnumerable<TSource> ExceptBy<TSource, TKey>(this IEnumerable<TSource> source, IEnumerable<TSource> target, Func<TSource, TKey> keySelector)
        {
            var seenTargets = new HashSet<TKey>();

            target.ForEach(item => seenTargets.Add(keySelector(item)));

            var result = source.Where(element =>
            {
                var added = seenTargets.Add(keySelector(element));

                if (added)
                {
                    seenTargets.Remove(keySelector(element));
                }

                return added;
            });

            return result;
        }

        #region IEnumerable<object> 转换成指定类型的集合

        private static readonly IDictionary<string, EnumerableAnonymousObject> _anonymousObjectsOfType = new Dictionary<string, EnumerableAnonymousObject>();

        public static object OfType(this IEnumerable<object> source, Type elementType)
        {
            var key = elementType.FullName;

            if (!_anonymousObjectsOfType.ContainsKey(key))
            {
                _anonymousObjectsOfType[key] = (EnumerableAnonymousObject)Activator.CreateInstance(typeof(EnumerableAnonymousObject<>).MakeGenericType(elementType));
            }

            return _anonymousObjectsOfType[key].ToList(source);
        }

        #endregion IEnumerable<object> 转换成指定类型的集合
    }

    #region 匿名对象集合转换

    internal abstract class EnumerableAnonymousObject
    {
        public abstract object ToList(IEnumerable<object> items);
    }

    /// <summary>
    /// 匿名对象集合转换。
    /// </summary>
    /// <typeparam name="TElementType"></typeparam>
    internal class EnumerableAnonymousObject<TElementType> : EnumerableAnonymousObject
    {
        public override object ToList(IEnumerable<object> items)
        {
            return items.OfType<TElementType>();
        }
    }

    #endregion 匿名对象集合转换
}