using AutoLua.Core.Extensions;
using System;
using System.Collections.Generic;

namespace AutoLua.Core.Caching
{
    /// <summary>
    /// 默认缓存管理器。
    /// </summary>
    public class DefaultCacheManager : ICacheManager
    {
        /// <summary>
        /// 静态字典缓存数据。
        /// </summary>
        private volatile IDictionary<string, CacheEntity> _cache = new Dictionary<string, CacheEntity>();

        /// <summary>
        /// 锁定对象。
        /// </summary>
        private static readonly object LockObject = new object();

        /// <inheritdoc />
        /// <summary>
        /// 获得所有 Key。
        /// </summary>
        public IEnumerable<string> Keys => _cache.Keys;

        /// <summary>
        /// 获取缓存值。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="key">键对象</param>
        /// <returns>值对象</returns>
        public TValue Get<TValue>(string key)
        {
            CacheEntity cache = null;

            lock (_cache)
            {
                if (_cache.ContainsKey(key))
                    cache = _cache[key];

                if (cache == null)
                    return default;

                if (cache.ExpiredTime < DateTime.UtcNow)
                {
                    Remove(key);
                    return default;
                }

                ClearExpired();

                return (TValue)cache.Value;
            }
        }

        /// <summary>
        /// 获取缓存值，如果不存在，则添加缓存。
        /// 如果存在则更新过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="validFor"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Get<TValue>(string key, TimeSpan validFor)
        {
            CacheEntity cache = null;

            lock (_cache)
            {
                if (_cache.ContainsKey(key))
                    cache = _cache[key];

                if (cache == null)
                    return default;

                if (cache.ExpiredTime < DateTime.UtcNow)
                {
                    Remove(key);
                    return default;
                }

                cache.ExpiredTime = DateTime.UtcNow.Add(validFor);

                this.ClearExpired();

                return (TValue)cache.Value;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 获取缓存值，如果不存在，则添加缓存。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="key">键对象</param>
        /// <param name="acquire">获得值对象的委托。</param>
        /// <returns>值对象。</returns>
        public TValue GetOrAdd<TValue>(string key, Func<TValue> acquire)
        {
            lock (_cache)
            {
                var result = Get<TValue>(key);

                if (result != null)
                    return result;

                result = acquire();

                if (result != null)
                    Set(key, result);

                return result;
            }
        }

        /// <summary>
        /// 获取缓存值，如果不存在，则添加缓存。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="key">键对象。</param>
        /// <param name="acquire">获得值对象的委托。</param>
        /// <param name="validFor"></param>
        /// <returns>值对象。</returns>
        public TValue GetOrAdd<TValue>(string key, Func<TValue> acquire, TimeSpan validFor)
        {
            lock (_cache)
            {
                var result = Get<TValue>(key);

                if (result != null)
                    return result;

                result = acquire();

                if (result != null)
                    Set(key, result, validFor);

                return result;
            }
        }

        /// <summary>
        /// 增加缓存值。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="key">键对象</param>
        /// <param name="value">值对象</param>
        public void Set<TValue>(string key, TValue value)
        {
            Set(key, value, new TimeSpan(0, 0, 20, 0));
        }

        /// <summary>
        /// 增加缓存值。
        /// </summary>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="key">键对象。</param>
        /// <param name="value">值对象。</param>
        /// <param name="validFor">有效时长。</param>
        /// <exception cref="ArgumentNullException">key 不可以是空的。</exception>
        public void Set<TValue>(string key, TValue value, TimeSpan validFor)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key), "键对象不可以是空的。");

            lock (_cache)
            {
                var cache = new CacheEntity
                {
                    Value = value,
                    ExpiredTime = DateTime.UtcNow.Add(validFor)
                };

                _cache[key] = cache;
            }
        }

        /// <summary>
        /// 移除缓存值。
        /// </summary>
        /// <param name="key">键对象。</param>
        public void Remove(string key)
        {
            lock (_cache)
            {
                if (_cache.ContainsKey(key))
                {
                    _cache.Remove(key);
                }
            }
        }

        /// <summary>
        /// 移除缓存值。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="key">键对象</param>
        /// <param name="result">值对象</param>
        public void Remove<TValue>(string key, out TValue result)
        {
            result = default(TValue);

            lock (_cache)
            {
                if (_cache.ContainsKey(key))
                {
                    result = (TValue)_cache[key].Value;
                    _cache.Remove(key);
                }
            }
        }

        /// <summary>
        /// 清空缓存。
        /// </summary>
        public void Clear()
        {
            lock (_cache)
            {
                _cache.Clear();
            }
        }

        private void ClearExpired()
        {
            lock (_cache)
            {
                _cache.RemoveWhere(x => x.Value.ExpiredTime < DateTime.UtcNow);
            }
        }
    }
}