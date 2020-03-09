using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using AutoLua.Droid.Utils;
using AutoLua.Droid.Views.RecyclerViews;
using AutoLua.Droid.Views.RecyclerViews.Adapters;
using AutoLua.Droid.Views.RecyclerViews.Swipes;

namespace AutoLua.Droid.Base
{
    /// <summary>
    /// 包含适配器的父类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseRVActivity<T> : BaseActivity, IOnLoadMoreListener, IOnRefreshListener,
        RecyclerArrayAdapter<T>.IOnItemClickListener where T : class
    {
        protected EasyRecyclerView mRecyclerView;
        protected RecyclerArrayAdapter<T> mAdapter;


        public void OnLoadMore()
        {
            throw new System.NotImplementedException();
        }

        public void OnRefresh()
        {
            throw new System.NotImplementedException();
        }

        public void OnItemClick(int position)
        {
            throw new System.NotImplementedException();
        }
    }
}