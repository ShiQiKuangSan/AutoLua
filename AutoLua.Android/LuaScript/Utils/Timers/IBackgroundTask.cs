using System;

namespace AutoLua.Droid.LuaScript.Utils.Timers
{
    /// <inheritdoc />
    /// <summary>
    /// 后台任务接口。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public interface IBackgroundTask : IDisposable
    {
        /// <summary>
        /// 标识。
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 名称。
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 是否启动。
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// 启动。
        /// </summary>
        void Start();

        /// <summary>
        /// 终止。
        /// </summary>
        void Stop();

        /// <summary>
        /// 设置时钟周期。
        /// </summary>
        /// <param name="interval">时钟周期。(单位：毫秒)</param>
        void SetInterval(double interval);
    }
}