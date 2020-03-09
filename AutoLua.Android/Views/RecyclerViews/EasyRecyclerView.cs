using System.Collections.Generic;

using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using AndroidResource = Android.Resource;
using static Android.Support.V7.Widget.RecyclerView;
using Android.Views.Animations;
using AutoLua.Droid.Utils;
using AutoLua.Droid.Views.RecyclerViews.Adapters;
using AutoLua.Droid.Views.RecyclerViews.Decorations;
using AutoLua.Droid.Views.RecyclerViews.Swipes;
using SwipeRefreshLayout = AutoLua.Droid.Views.RecyclerViews.Swipes.SwipeRefreshLayout;

namespace AutoLua.Droid.Views.RecyclerViews
{
    [Register("AutoLua.Droid.Views.RecyclerViews.EasyRecyclerView")]
    public class EasyRecyclerView : FrameLayout
    {
        public const string TAG = "EasyRecyclerView";
        public static bool DEBUG = false;
        protected RecyclerView mRecycler;
        protected TextView tipView;
        protected ViewGroup mProgressView;
        protected ViewGroup mEmptyView;
        protected ViewGroup mErrorView;
        private int _progressId;
        private int _emptyId;
        private int _errorId;

        protected bool mClipToPadding;
        protected int mPadding;
        protected int mPaddingTop;
        protected int mPaddingBottom;
        protected int mPaddingLeft;
        protected int mPaddingRight;
        protected int mScrollbarStyle;
        protected int mScrollbar;

        protected OnScrollListener mInternalOnScrollListener;
        protected OnScrollListener mExternalOnScrollListener;

        protected SwipeRefreshLayout mPtrLayout;
        protected IOnRefreshListener mRefreshListener;

        public readonly List<ItemDecoration> decorations = new List<ItemDecoration>();


        public SwipeRefreshLayout GetSwipeToRefresh()
        {
            return mPtrLayout;
        }

        public RecyclerView GetRecyclerView()
        {
            return mRecycler;
        }

        public EasyRecyclerView(Context context, IAttributeSet attrs = null)
                : this(context, attrs, 0)
        {

        }

        public EasyRecyclerView(Context context, IAttributeSet attrs, int defStyle)
                : base(context, attrs, defStyle)
        {
            if (attrs != null)
                InitAttrs(attrs);
            
            InitView();
        }

        protected void InitAttrs(IAttributeSet attrs)
        {
            var a = Context.ObtainStyledAttributes(attrs, Resource.Styleable.SuperRecyclerView);
            try
            {
                mClipToPadding = a.GetBoolean(Resource.Styleable.SuperRecyclerView_recyclerClipToPadding, false);
                mPadding = (int)a.GetDimension(Resource.Styleable.SuperRecyclerView_recyclerPadding, -1.0f);
                mPaddingTop = (int)a.GetDimension(Resource.Styleable.SuperRecyclerView_recyclerPaddingTop, 0.0f);
                mPaddingBottom = (int)a.GetDimension(Resource.Styleable.SuperRecyclerView_recyclerPaddingBottom, 0.0f);
                mPaddingLeft = (int)a.GetDimension(Resource.Styleable.SuperRecyclerView_recyclerPaddingLeft, 0.0f);
                mPaddingRight = (int)a.GetDimension(Resource.Styleable.SuperRecyclerView_recyclerPaddingRight, 0.0f);
                mScrollbarStyle = a.GetInteger(Resource.Styleable.SuperRecyclerView_scrollbarStyle, -1);
                mScrollbar = a.GetInteger(Resource.Styleable.SuperRecyclerView_scrollbars, -1);
                
                _emptyId = a.GetResourceId(Resource.Styleable.SuperRecyclerView_layout_empty, 0);
                _progressId = a.GetResourceId(Resource.Styleable.SuperRecyclerView_layout_progress, 0);
                _errorId = a.GetResourceId(Resource.Styleable.SuperRecyclerView_layout_error, Resource.Layout.common_net_error_view);
            }
            finally
            {
                a.Recycle();
            }
        }

        private void InitView()
        {
            if (IsInEditMode)
            {
                return;
            }
            //生成主View
            var v = LayoutInflater.From(Context).Inflate(Resource.Layout.common_recyclerview, this);
            mPtrLayout = (SwipeRefreshLayout)v.FindViewById(Resource.Id.ptr_layout);
            mPtrLayout.Enabled = false;

            mProgressView = (ViewGroup)v.FindViewById(Resource.Id.progress);
            
            if (_progressId != 0)
                LayoutInflater.From(Context).Inflate(_progressId, mProgressView);
            
            mEmptyView = (ViewGroup)v.FindViewById(Resource.Id.empty);
            
            if (_emptyId != 0)
                LayoutInflater.From(Context).Inflate(_emptyId, mEmptyView);
            
            mErrorView = (ViewGroup)v.FindViewById(Resource.Id.error);
            
            if (_errorId != 0) 
                LayoutInflater.From(Context).Inflate(_errorId, mErrorView);
            
            InitRecyclerView(v);
        }

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            return mPtrLayout.DispatchTouchEvent(ev);
        }

        /**
         * @param left
         * @param top
         * @param right
         * @param bottom
         */
        public void SetRecyclerPadding(int left, int top, int right, int bottom)
        {
            mPaddingLeft = left;
            mPaddingTop = top;
            mPaddingRight = right;
            mPaddingBottom = bottom;
            mRecycler.SetPadding(mPaddingLeft, mPaddingTop, mPaddingRight, mPaddingBottom);
        }

        public void setClipToPadding(bool isClip)
        {
            mRecycler.SetClipToPadding(isClip);
        }


        public void SetEmptyView(View emptyView)
        {
            mEmptyView.RemoveAllViews();
            mEmptyView.AddView(emptyView);
        }

        public void SetProgressView(View progressView)
        {
            mProgressView.RemoveAllViews();
            mProgressView.AddView(progressView);
        }

        public void SetErrorView(View errorView)
        {
            mErrorView.RemoveAllViews();
            mErrorView.AddView(errorView);
        }

        public void SetEmptyView(int emptyView)
        {
            mEmptyView.RemoveAllViews();
            LayoutInflater.From(Context).Inflate(emptyView, mEmptyView);
        }

        public void SetProgressView(int progressView)
        {
            mProgressView.RemoveAllViews();
            LayoutInflater.From(Context).Inflate(progressView, mProgressView);
        }

        public void SetErrorView(int errorView)
        {
            mErrorView.RemoveAllViews();
            LayoutInflater.From(Context).Inflate(errorView, mErrorView);
        }

        public void ScrollToPosition(int position)
        {
            GetRecyclerView().ScrollToPosition(position);
        }

        /**
         * Implement this method to customize the AbsListView
         */
        protected void InitRecyclerView(View view)
        {
            mRecycler = (RecyclerView)view.FindViewById(AndroidResource.Id.List);
            tipView = (TextView)view.FindViewById(Resource.Id.tvTip);
            SetItemAnimator(null);
            
            if (mRecycler == null)
                return;
            
            mRecycler.HasFixedSize = true;
            mRecycler.SetClipToPadding(mClipToPadding);
            mInternalOnScrollListener = new CustomOnScrollListener(this);
            mRecycler.AddOnScrollListener(mInternalOnScrollListener);

            if (mPadding != -1.0f)
            {
                mRecycler.SetPadding(mPadding, mPadding, mPadding, mPadding);
            }
            else
            {
                mRecycler.SetPadding(mPaddingLeft, mPaddingTop, mPaddingRight, mPaddingBottom);
            }
            if (mScrollbarStyle != -1)
            {
                mRecycler.ScrollBarStyle = (ScrollbarStyles)mScrollbarStyle;
            }
            switch (mScrollbar)
            {
                case 0:
                    VerticalScrollBarEnabled = false;
                    break;
                case 1:
                    HorizontalScrollBarEnabled = false;
                    break;
                case 2:
                    VerticalScrollBarEnabled = false;
                    HorizontalScrollBarEnabled = false;
                    break;
            }
        }

        public override bool HorizontalFadingEdgeEnabled
        {
            get => mRecycler.HorizontalFadingEdgeEnabled;
            set => mRecycler.HorizontalFadingEdgeEnabled = value;
        }

        /**
         * Set the layout manager to the recycler
         *
         * @param manager
         */
        public void SetLayoutManager(RecyclerView.LayoutManager manager)
        {
            mRecycler.SetLayoutManager(manager);
        }

        /**
         * Set the ItemDecoration to the recycler
         *
         * @param color
         * @param height
         * @param paddingLeft
         * @param paddingRight
         */
        public void SetItemDecoration(int color, int height, int paddingLeft, int paddingRight)
        {
            var itemDecoration = new DividerDecoration(color, height, paddingLeft, paddingRight);
            itemDecoration.SetDrawLastItem(false);
            decorations.Add(itemDecoration);
            mRecycler.AddItemDecoration(itemDecoration);
        }


        public class EasyDataObserver : AdapterDataObserver
        {
            private readonly EasyRecyclerView _recyclerView;

            public EasyDataObserver(EasyRecyclerView recyclerView)
            {
                _recyclerView = recyclerView;
            }

            public override void OnItemRangeChanged(int positionStart, int itemCount)
            {
                base.OnItemRangeChanged(positionStart, itemCount);
                Update();
            }

            public override void OnItemRangeInserted(int positionStart, int itemCount)
            {
                base.OnItemRangeInserted(positionStart, itemCount);
                Update();
            }

            public override void OnItemRangeRemoved(int positionStart, int itemCount)
            {
                base.OnItemRangeRemoved(positionStart, itemCount);
                Update();
            }

            public override void OnItemRangeMoved(int fromPosition, int toPosition, int itemCount)
            {
                base.OnItemRangeMoved(fromPosition, toPosition, itemCount);

                Update();
            }

            public override void OnChanged()
            {
                base.OnChanged();
                Update();

            }

            //自动更改Container的样式
            private void Update()
            {
                Log("update");
                int count;
                var adapterObj = _recyclerView.GetAdapter();
                var adapterType = adapterObj.GetType();
                if (adapterType.IsAssignableFrom(typeof(RecyclerArrayAdapter<>)))
                {
                    count = (int)adapterType.GetMethod("getCount").Invoke(adapterObj, null);
                }
                else
                {
                    count = _recyclerView.GetAdapter().ItemCount;
                }

                switch (count)
                {
                    case 0 when !NetworkUtils.IsAvailable(_recyclerView.Context):
                        _recyclerView.ShowError();
                        return;
                    case 0 when adapterType.GetMethod("getHeaderCount") != null && (int)adapterType.GetMethod("getHeaderCount").Invoke(adapterObj, null) == 0:
                        Log("no data:" + "show empty");
                        _recyclerView.ShowEmpty();
                        break;
                    default:
                        Log("has data");
                        _recyclerView.ShowRecycler();
                        break;
                }
            }
        }

        /**
         * 设置适配器，关闭所有副view。展示recyclerView
         * 适配器有更新，自动关闭所有副view。根据条数判断是否展示EmptyView
         *
         * @param adapter
         */
        public void SetAdapter(RecyclerView.Adapter adapter)
        {
            mRecycler.SetAdapter(adapter);
            adapter.RegisterAdapterDataObserver(new EasyDataObserver(this));
            ShowRecycler();
        }

        /**
         * 设置适配器，关闭所有副view。展示进度条View
         * 适配器有更新，自动关闭所有副view。根据条数判断是否展示EmptyView
         *
         * @param adapter
         */
        public void SetAdapterWithProgress(RecyclerView.Adapter adapter)
        {
            mRecycler.SetAdapter(adapter);
            adapter.RegisterAdapterDataObserver(new EasyDataObserver(this));
            var adapterType = adapter.GetType();
            //只有Adapter为空时才显示ProgressView
            
            if (adapterType.IsAssignableFrom(typeof(RecyclerArrayAdapter<>)))
            {
                if ((int)adapterType.GetMethod("getCount").Invoke(adapter, null) == 0)
                {
                    ShowProgress();
                }
                else
                {
                    ShowRecycler();
                }
            }
            else
            {
                if (adapter.ItemCount == 0)
                {
                    ShowProgress();
                }
                else
                {
                    ShowRecycler();
                }
            }
        }

        /**
         * Remove the adapter from the recycler
         */
        public void Clear()
        {
            mRecycler.SetAdapter(null);
        }


        private void HideAll()
        {
            mEmptyView.Visibility = ViewStates.Gone;
            mProgressView.Visibility = ViewStates.Gone;
            mErrorView.Visibility = ViewStates.Gone;
            mRecycler.Visibility = ViewStates.Invisible;
        }


        public void ShowError()
        {
            Log("showError");
            if (mErrorView.ChildCount > 0)
            {
                HideAll();
                mErrorView.Visibility = ViewStates.Visible;
            }
            else
            {
                ShowRecycler();
            }
        }

        public void ShowEmpty()
        {
            Log("showEmpty");
            if (mEmptyView.ChildCount > 0)
            {
                HideAll();
                mEmptyView.Visibility = ViewStates.Visible;
            }
            else
            {
                ShowRecycler();
            }
        }


        public void ShowProgress()
        {
            Log("showProgress");
            if (mProgressView.ChildCount > 0)
            {
                HideAll();
                mProgressView.Visibility = ViewStates.Visible;
            }
            else
            {
                ShowRecycler();
            }
        }


        public void ShowRecycler()
        {
            Log("showRecycler");
            HideAll();
            mRecycler.Visibility = ViewStates.Visible;
        }

        public void ShowTipViewAndDelayClose(string tip)
        {
            tipView.Text = tip;
            Animation mShowAction = new TranslateAnimation(Dimension.RelativeToSelf, 0.0f,
                    Dimension.RelativeToSelf, 0.0f, Dimension.RelativeToSelf,
                    -1.0f, Dimension.RelativeToSelf, 0.0f);
            mShowAction.Duration = 500;
            tipView.StartAnimation(mShowAction);
            tipView.Visibility = ViewStates.Visible;

            tipView.PostDelayed(() =>
            {
                Animation mHiddenAction = new TranslateAnimation(Dimension.RelativeToSelf,
                            0.0f, Dimension.RelativeToSelf, 0.0f,
                            Dimension.RelativeToSelf, 0.0f, Dimension.RelativeToSelf,
                            -1.0f);
                mHiddenAction.Duration = 500;
                tipView.StartAnimation(mHiddenAction);
                tipView.Visibility = ViewStates.Gone;
            }, 2200);
        }

        public void ShowTipView(string tip)
        {
            tipView.Text = tip;
            Animation mShowAction = new TranslateAnimation(Dimension.RelativeToSelf, 0.0f,
                Dimension.RelativeToSelf, 0.0f, Dimension.RelativeToSelf,
                -1.0f, Dimension.RelativeToSelf, 0.0f);
            mShowAction.Duration = 500;
            tipView.StartAnimation(mShowAction);
            tipView.Visibility = ViewStates.Visible;
        }

        public void HideTipView(long delayMillis)
        {
            tipView.PostDelayed(() =>
            {
                Animation mHiddenAction = new TranslateAnimation(Dimension.RelativeToSelf,
                            0.0f, Dimension.RelativeToSelf, 0.0f,
                            Dimension.RelativeToSelf, 0.0f, Dimension.RelativeToSelf,
                            -1.0f);
                mHiddenAction.Duration = 500;
                tipView.StartAnimation(mHiddenAction);
                tipView.Visibility = ViewStates.Gone;
            }, delayMillis);
        }

        public void SetTipViewText(string tip)
        {
            if (!IsTipViewVisible())
                ShowTipView(tip);
            else
                tipView.Text = tip;
        }

        public bool IsTipViewVisible()
        {
            return tipView.Visibility == ViewStates.Visible;
        }


        /**
         * Set the listener when refresh is triggered and enable the SwipeRefreshLayout
         *
         * @param listener
         */
        public void SetRefreshListener(IOnRefreshListener listener)
        {
            mPtrLayout.Enabled = true;
            mPtrLayout.SetOnRefreshListener(listener);
            this.mRefreshListener = listener;
        }

        public void SetRefreshing(bool isRefreshing)
        {
            mPtrLayout.Post(() =>
            {
                if (isRefreshing)
                { // 避免刷新的loadding和progressview 同时显示
                    mProgressView.Visibility = ViewStates.Gone;
                }
                mPtrLayout.SetRefreshing(isRefreshing);
            });
        }

        public void SetRefreshing(bool isRefreshing, bool isCallbackListener)
        {
            mPtrLayout.Post(() =>
            {
                mPtrLayout.SetRefreshing(isRefreshing);
                if (isRefreshing && isCallbackListener)
                {
                    mRefreshListener?.OnRefresh();
                }
            });
        }

        /**
         * Set the colors for the SwipeRefreshLayout states
         *
         * @param colRes
         */
        public void SetRefreshingColorResources(int[] colRes)
        {
            mPtrLayout.SetColorSchemeResources(colRes);
        }

        /**
         * Set the colors for the SwipeRefreshLayout states
         *
         * @param col
         */
        public void SetRefreshingColor(int[] col)
        {
            mPtrLayout.SetColorSchemeColors(col);
        }

        /**
         * Set the scroll listener for the recycler
         *
         * @param listener
         */
        public void setOnScrollListener(RecyclerView.OnScrollListener listener)
        {
            mExternalOnScrollListener = listener;
        }

        /**
         * Add the onItemTouchListener for the recycler
         *
         * @param listener
         */
        public void addOnItemTouchListener(RecyclerView.IOnItemTouchListener listener)
        {
            mRecycler.AddOnItemTouchListener(listener);
        }

        /**
         * Remove the onItemTouchListener for the recycler
         *
         * @param listener
         */
        public void removeOnItemTouchListener(RecyclerView.IOnItemTouchListener listener)
        {
            mRecycler.RemoveOnItemTouchListener(listener);
        }

        /**
         * @return the recycler adapter
         */
        public RecyclerView.Adapter GetAdapter()
        {
            return mRecycler.GetAdapter();
        }


        public void setOnTouchListener(IOnTouchListener listener)
        {
            mRecycler.SetOnTouchListener(listener);
        }

        public void SetItemAnimator(ItemAnimator animator)
        {
            mRecycler.SetItemAnimator(animator);
        }

        public void AddItemDecoration(ItemDecoration itemDecoration)
        {
            mRecycler.AddItemDecoration(itemDecoration);
        }

        public void AddItemDecoration(ItemDecoration itemDecoration, int index)
        {
            mRecycler.AddItemDecoration(itemDecoration, index);
        }

        public void RemoveItemDecoration(ItemDecoration itemDecoration)
        {
            mRecycler.RemoveItemDecoration(itemDecoration);
        }

        public void RemoveAllItemDecoration()
        {
            foreach (var decoration in decorations)
            {
                mRecycler.RemoveItemDecoration(decoration);
            }
        }


        /**
         * @return inflated error view or null
         */
        public View GetErrorView()
        {
            return mErrorView.ChildCount > 0 ? mErrorView.GetChildAt(0) : null;
        }

        /**
         * @return inflated progress view or null
         */
        public View GetProgressView()
        {
            return mProgressView.ChildCount > 0 ? mProgressView.GetChildAt(0) : null;
        }


        /**
         * @return inflated empty view or null
         */
        public View GetEmptyView()
        {
            return mEmptyView.ChildCount > 0 ? mEmptyView.GetChildAt(0) : null;
        }

        private static void Log(string content)
        {
            if (DEBUG)
            {
                Android.Util.Log.Info(TAG, content);
            }
        }

        private class CustomOnScrollListener : OnScrollListener
        {
            private readonly EasyRecyclerView _easyRecyclerView;

            public CustomOnScrollListener(EasyRecyclerView easyRecyclerView)
            {
                _easyRecyclerView = easyRecyclerView;
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                base.OnScrolled(recyclerView, dx, dy);
                _easyRecyclerView.mExternalOnScrollListener?.OnScrolled(recyclerView, dx, dy);
            }

            public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
            {
                base.OnScrollStateChanged(recyclerView, newState);
                _easyRecyclerView.mExternalOnScrollListener?.OnScrollStateChanged(recyclerView, newState);
            }
        }
    }
}