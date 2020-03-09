using Android.Views;

namespace AutoLua.Droid.Views.RecyclerViews.Adapters
{
    /// <summary>
    /// 事件委托接口
    /// </summary>
    public interface IEventDelegate
    {
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="length"></param>
        void AddData(int length);

        /// <summary>
        /// 清空
        /// </summary>
        void Clear();

        /// <summary>
        /// 停止加载更多
        /// </summary>
        void StopLoadMore();

        /// <summary>
        /// 暂停加载更多
        /// </summary>
        void PauseLoadMore();

        /// <summary>
        /// 重新加载更多
        /// </summary>
        void ResumeLoadMore();

        /// <summary>
        /// 设置更多加载事件
        /// </summary>
        /// <param name="view">视图</param>
        /// <param name="listener">加载更多事件</param>
        void SetMore(View view, IOnLoadMoreListener listener);

        /// <summary>
        /// 不在加载更多
        /// </summary>
        /// <param name="view"></param>
        void SetNoMore(View view);

        /// <summary>
        /// 设置错误更多
        /// </summary>
        /// <param name="view"></param>
        void SetErrorMore(View view);
    }
}