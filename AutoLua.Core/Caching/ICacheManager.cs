using System;
using System.Collections.Generic;

namespace AutoLua.Core.Caching
{
    /// <summary>
    /// 缓存管理器
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// 获得所有 Key。
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// 获取缓存值。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="key">键对象。</param>
        /// <returns>值对象。</returns>
        TValue Get<TValue>(string key);

        /// <summary>
        /// 获取缓存值，如果不存在，则添加缓存。
        /// 如果存在则更新过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="validFor"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        TValue Get<TValue>(string key, TimeSpan validFor);

        /// <summary>
        /// 获取缓存值，如果不存在，则添加缓存。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="key">键对象。</param>
        /// <param name="acquire">获得值对象的委托。</param>
        /// <returns>值对象。</returns>
        TValue GetOrAdd<TValue>(string key, Func<TValue> acquire);

        /// <summary>
        /// 获取缓存值，如果不存在，则添加缓存。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="key">键对象。</param>
        /// <param name="acquire">获得值对象的委托。</param>
        /// <param name="validFor"></param>
        /// <returns>值对象。</returns>
        TValue GetOrAdd<TValue>(string key, Func<TValue> acquire, TimeSpan validFor);

        /// <summary>
        /// 增加缓存值。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="key">键对象。</param>
        /// <param name="value">值对象。</param>
        void Set<TValue>(string key, TValue value);

        /// <summary>
        /// 增加缓存值。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="key">键对象。</param>
        /// <param name="value">值对象。</param>
        /// <param name="validFor">有效时长。</param>
        void Set<TValue>(string key, TValue value, TimeSpan validFor);

        /// <summary>
        /// 移除缓存值。
        /// </summary>
        /// <param name="key">键对象。</param>
        void Remove(string key);

        /// <summary>
        /// 移除缓存值。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="key">键对象。</param>
        /// <param name="result">值对象。</param>
        void Remove<TValue>(string key, out TValue result);

        /// <summary>
        /// 清空缓存。
        /// </summary>
        void Clear();
    }
}