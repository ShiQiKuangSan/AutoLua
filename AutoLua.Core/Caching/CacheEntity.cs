using System;

namespace AutoLua.Core.Caching
{
    /// <summary>
    /// 缓存实体。
    /// </summary>
    public class CacheEntity
    {
        /// <summary>
        /// 缓存值。
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 过期时间（UTC）
        /// </summary>
        public DateTime ExpiredTime { get; set; }
    }
}