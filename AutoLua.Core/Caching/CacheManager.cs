using System;

namespace AutoLua.Core.Caching
{
    /// <summary>
    /// 缓存管理器。
    /// </summary>
    public class CacheManager
    {
        #region 私有对象

        /// <summary>
        /// 缓存管理器实例。
        /// </summary>
        private static readonly CacheManager Instance = new CacheManager();

        /// <summary>
        /// 锁对象。
        /// </summary>
        private static readonly object Lock = new object();

        #endregion Members

        #region 单列

        /// <summary>
        /// 当前缓存管理器。
        /// </summary>
        public static ICacheManager Current
        {
            get
            {
                lock (Lock)
                {
                    return Instance.InnerCurrent;
                }
            }
        }

        /// <summary>
        /// 当前缓存管理器。
        /// </summary>
        private ICacheManager InnerCurrent { get; set; } = new DefaultCacheManager();

        #endregion Properties

        #region 公用方法

        /// <summary>
        /// 设置当前使用的缓存方式。
        /// </summary>
        /// <param name="cacheManager">缓存管理器。</param>
        public static void SetCacheManager(ICacheManager cacheManager)
        {
            lock (Lock)
            {
                Instance.InnerSetCacheManager(cacheManager);
            }
        }

        #endregion Public Methods

        #region 私有方法

        /// <summary>
        /// 设置当前使用的缓存方式。
        /// </summary>
        /// <param name="cacheManager">缓存管理器。</param>
        private void InnerSetCacheManager(ICacheManager cacheManager)
        {
            InnerCurrent = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        #endregion Pricate Method
    }
}