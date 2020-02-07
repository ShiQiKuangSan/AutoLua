namespace AutoLua.Droid.LuaScript.Utils.Medias
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public enum MediaPlayerState
    {
        /// <summary>
        /// 未初始化
        /// </summary>
        NotInitialized,

        /// <summary>
        /// 准备中
        /// </summary>
        Preparing,

        /// <summary>
        /// 已准备
        /// </summary>
        Prepared,

        /// <summary>
        /// 开始
        /// </summary>
        Start,

        /// <summary>
        /// 暂停
        /// </summary>
        Paused,

        /// <summary>
        /// 停止
        /// </summary>
        Stopped,
            
        /// <summary>
        /// 发行
        /// </summary>
        Released,
    }
}