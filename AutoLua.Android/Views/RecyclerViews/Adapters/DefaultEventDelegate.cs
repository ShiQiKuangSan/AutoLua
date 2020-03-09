using Android.Views;
using Android.Widget;

namespace AutoLua.Droid.Views.RecyclerViews.Adapters
{
    /// <summary>
    /// 默认的事件委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultEventDelegate<T> : Java.Lang.Object, IEventDelegate where T : class
    {
        private readonly RecyclerArrayAdapter<T> _adapter;
        private readonly EventFooter _footer;

        private IOnLoadMoreListener _onLoadMoreListener;

        private bool _hasData;
        private bool _isLoadingMore;

        private bool _hasMore;
        private bool _hasNoMore;
        private bool _hasError;

        private int _status = StatusInitial;
        private const int StatusInitial = 291;
        private const int StatusMore = 260;
        private const int StatusNomore = 408;
        private const int StatusError = 732;

        public DefaultEventDelegate(RecyclerArrayAdapter<T> adapter)
        {
            _adapter = adapter;
            _footer = new EventFooter(this);
            adapter.AddFooter(_footer);
        }

        public void OnMoreViewShowed()
        {
            Log("onMoreViewShowed");
            if (!_isLoadingMore && _onLoadMoreListener != null)
            {
                _isLoadingMore = true;
                _onLoadMoreListener.OnLoadMore();
            }
        }

        public void OnErrorViewShowed()
        {
            ResumeLoadMore();
        }

        public void AddData(int length)
        {
            Log("addData" + length);
            if (_hasMore)
            {
                if (length == 0)
                {
                    //当添加0个时，认为已结束加载到底
                    if (_status == StatusInitial || _status == StatusMore)
                    {
                        _footer.showNoMore();
                    }
                }
                else
                {
                    //当Error或初始时。添加数据，如果有More则还原。
                    if (_hasMore && (_status == StatusInitial || _status == StatusError))
                    {
                        _footer.showMore();
                    }

                    _hasData = true;
                }
            }
            else
            {
                if (_hasNoMore)
                {
                    _footer.showNoMore();
                    _status = StatusNomore;
                }
            }

            _isLoadingMore = false;
        }

        public void Clear()
        {
            Log("clear");
            _hasData = false;
            _status = StatusInitial;
            _footer.hide();
            _isLoadingMore = false;
        }

        public void PauseLoadMore()
        {
            Log("pauseLoadMore");
            _footer.showError();
            _status = StatusError;
            _isLoadingMore = false;
        }

        public void ResumeLoadMore()
        {
            _isLoadingMore = false;
            _footer.showMore();
            OnMoreViewShowed();
        }

        public void SetErrorMore(View view)
        {
            _footer.SetErrorView(view);
            _hasError = true;
            Log("setErrorMore");
        }

        public void SetMore(View view, IOnLoadMoreListener listener)
        {
            _footer.SetMoreView(view);
            _onLoadMoreListener = listener;
            _hasMore = true;
            Log("setMore");
        }

        public void SetNoMore(View view)
        {
            _footer.SetNoMoreView(view);
            _hasNoMore = true;
            Log("setNoMore");
        }

        public void StopLoadMore()
        {
            Log("stopLoadMore");
            _footer.showNoMore();
            _status = StatusNomore;
            _isLoadingMore = false;
        }

        private class EventFooter : RecyclerArrayAdapter<T>.ITemView
        {
            private readonly DefaultEventDelegate<T> _eventDelegate;

            private readonly FrameLayout _container;
            private View _moreView;
            private View _noMoreView;
            private View _errorView;

            private int _flag = Hide;
            public const int Hide = 0;
            public const int ShowMore = 1;
            public const int ShowError = 2;
            public const int ShowNoMore = 3;


            public EventFooter(DefaultEventDelegate<T> eventDelegate)
            {
                _eventDelegate = eventDelegate;
                _container = new FrameLayout(_eventDelegate._adapter.GetContext())
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.WrapContent)
                };
            }

            public View OnCreateView(ViewGroup parent)
            {
                Log("onCreateView");
                return _container;
            }

            public void OnBindView(View headerView)
            {
                Log("onBindView");
                switch (_flag)
                {
                    case ShowMore:
                        _eventDelegate.OnMoreViewShowed();
                        break;
                    case ShowError:
                        _eventDelegate.OnErrorViewShowed();
                        break;
                }
            }

            public void RefreshStatus()
            {
                if (_container == null) 
                    return;
                
                if (_flag == Hide)
                {
                    _container.Visibility = ViewStates.Gone;
                    return;
                }

                if (_container.Visibility != ViewStates.Visible) 
                    _container.Visibility = ViewStates.Visible;
                    
                View view = null;

                switch (_flag)
                {
                    case ShowMore:
                        view = _moreView;
                        break;
                    case ShowError:
                        view = _errorView;
                        break;
                    case ShowNoMore:
                        view = _noMoreView;
                        break;
                }

                if (view == null)
                {
                    hide();
                    return;
                }

                if (view.Parent == null) _container.AddView(view);
                for (var i = 0; i < _container.ChildCount; i++)
                {
                    if (_container.GetChildAt(i) == view) view.Visibility = ViewStates.Visible;
                    else _container.GetChildAt(i).Visibility = ViewStates.Gone;
                }
            }

            public void showError()
            {
                _flag = ShowError;
                RefreshStatus();
            }

            public void showMore()
            {
                _flag = ShowMore;
                RefreshStatus();
            }

            public void showNoMore()
            {
                _flag = ShowNoMore;
                RefreshStatus();
            }

            //初始化
            public void hide()
            {
                _flag = Hide;
                RefreshStatus();
            }

            public void SetMoreView(View moreView)
            {
                _moreView = moreView;
            }

            public void SetNoMoreView(View noMoreView)
            {
                _noMoreView = noMoreView;
            }

            public void SetErrorView(View errorView)
            {
                _errorView = errorView;
            }
        }

        private static void Log(string content)
        {
            if (EasyRecyclerView.DEBUG)
            {
                Android.Util.Log.Info(EasyRecyclerView.TAG, content);
            }
        }
    }
}