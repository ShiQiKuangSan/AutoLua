using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace AutoLua.Droid.Views.RecyclerViews.Adapters
{
    /// <summary>
    /// RecyclerView 适配器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RecyclerArrayAdapter<T> : RecyclerView.Adapter where T : class
    {

        /// <summary>
        /// 包含代表此ArrayAdapter数据的对象列表。
        /// 此列表的内容在文档中称为“数组”。
        /// </summary>
        protected List<T> objects;

        protected IEventDelegate eventDelegate;
        protected readonly List<ITemView> headers = new List<ITemView>();
        protected readonly List<ITemView> footers = new List<ITemView>();

        protected IOnItemClickListener mItemClickListener;
        protected IOnItemLongClickListener mItemLongClickListener;

        /// <summary>
        /// 观察者
        /// </summary>
        private RecyclerView.AdapterDataObserver _observer;

        public interface ITemView
        {
            /// <summary>
            /// 创建视图
            /// </summary>
            /// <param name="parent"></param>
            /// <returns></returns>
            View OnCreateView(ViewGroup parent);

            /// <summary>
            /// 绑定视图
            /// </summary>
            /// <param name="headerView"></param>
            void OnBindView(View headerView);
        }

        public class GridSpanSizeLookup : GridLayoutManager.SpanSizeLookup
        {
            private readonly int _maxCount;

            /// <summary>
            /// 适配器
            /// </summary>
            private readonly RecyclerArrayAdapter<T> _adapter;

            public GridSpanSizeLookup(RecyclerArrayAdapter<T> adapter, int maxCount)
            {
                _adapter = adapter;
                _maxCount = maxCount;
            }

            public override int GetSpanSize(int position)
            {
                if (_adapter.headers.Count != 0)
                {
                    if (position < _adapter.headers.Count)
                        return _maxCount;
                }

                if (_adapter.footers.Count == 0)
                    return 1;

                var i = position - _adapter.headers.Count - _adapter.objects.Count;

                return i >= 0 ? _maxCount : 1;
            }
        }

        /// <summary>
        /// 获取网格跨度大小
        /// </summary>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public GridSpanSizeLookup ObtainGridSpanSizeLookUp(int maxCount)
        {
            return new GridSpanSizeLookup(this, maxCount);
        }
        
        private readonly object _lock = new object();


        /// <summary>
        /// 指无论何时无论何时都必须调用 {@link #NotifyDataSetChanged()}
        /// </summary>
        private bool _notifyOnChange = true;

        private Context _context;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context">当前上下文</param>
        public RecyclerArrayAdapter(Context context)
        {
            Init(context, new List<T>());
        }

        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context">当前上下文</param>
        /// <param name="objects">要在ListView中表示的对象。</param>
        public RecyclerArrayAdapter(Context context, IEnumerable<T> objects)
        {
            Init(context, objects.ToList());
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context">当前上下文</param>
        /// <param name="objects">要在ListView中表示的对象。</param>
        public RecyclerArrayAdapter(Context context, List<T> objects)
        {
            Init(context, objects);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="objs"></param>
        private void Init(Context context, List<T> objs)
        {
            _context = context;
            
            lock (_lock)
            {
                objects = objs;
            }
        }


        /// <summary>
        /// 停止加载更多
        /// </summary>
        /// <exception cref="NullPointerException"></exception>
        public void StopMore()
        {
            if (eventDelegate == null)
                throw new NullPointerException("You should invoking setLoadMore() first");
            
            eventDelegate.StopLoadMore();
        }

        /// <summary>
        /// 暂停加载更多
        /// </summary>
        /// <exception cref="NullPointerException"></exception>
        public void PauseMore()
        {
            if (eventDelegate == null)
                throw new NullPointerException("You should invoking setLoadMore() first");
            
            eventDelegate.PauseLoadMore();
        }

        /// <summary>
        /// 重新加载更多
        /// </summary>
        /// <exception cref="NullPointerException"></exception>
        public void ResumeMore()
        {
            if (eventDelegate == null)
                throw new NullPointerException("You should invoking setLoadMore() first");
            
            eventDelegate.ResumeLoadMore();
        }


        /// <summary>
        /// 添加页眉
        /// </summary>
        /// <param name="view"></param>
        /// <exception cref="NullPointerException"></exception>
        public void AddHeader(ITemView view)
        {
            if (view == null) 
                throw new NullPointerException("ItemView can't be null");
            
            headers.Add(view);
            NotifyItemInserted(footers.Count - 1);
        }

        /// <summary>
        /// 添加页脚
        /// </summary>
        /// <param name="view"></param>
        /// <exception cref="NullPointerException"></exception>
        public void AddFooter(ITemView view)
        {
            if (view == null) throw new NullPointerException("ItemView can't be null");
            footers.Add(view);
            NotifyItemInserted(headers.Count + GetCount() + footers.Count - 1);
        }

        /// <summary>
        /// 移除所有页眉
        /// </summary>
        public void RemoveAllHeader()
        {
            var count = headers.Count;
            headers.Clear();
            NotifyItemRangeRemoved(0, count);
        }

        /// <summary>
        /// 移除所有页脚
        /// </summary>
        public void RemoveAllFooter()
        {
            var count = footers.Count();
            footers.Clear();
            NotifyItemRangeRemoved(headers.Count + GetCount(), count);
        }

        /// <summary>
        /// 获得页眉
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ITemView GetHeader(int index)
        {
            return headers[index];
        }

        /// <summary>
        /// 获得页脚
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ITemView GetFooter(int index)
        {
            return footers[index];
        }

        /// <summary>
        /// 获得页眉数量
        /// </summary>
        /// <returns></returns>
        public int GetHeaderCount()
        {
            return headers.Count;
        }

        /// <summary>
        /// 获得页脚数量
        /// </summary>
        /// <returns></returns>
        public int GetFooterCount()
        {
            return footers.Count;
        }

        /// <summary>
        /// 移除指定页眉
        /// </summary>
        /// <param name="view"></param>
        public void RemoveHeader(ITemView view)
        {
            var position = headers.IndexOf(view);
            headers.Remove(view);
            NotifyItemRemoved(position);
        }

        /// <summary>
        /// 移除指定页脚
        /// </summary>
        /// <param name="view"></param>
        public void RemoveFooter(ITemView view)
        {
            var position = headers.Count + GetCount() + footers.IndexOf(view);
            footers.Remove(view);
            NotifyItemRemoved(position);
        }


        /// <summary>
        /// 获得事件委托
        /// </summary>
        /// <returns></returns>
        private IEventDelegate GetEventDelegate()
        {
            if (eventDelegate == null) 
                eventDelegate = new DefaultEventDelegate<T>(this);
            
            return eventDelegate;
        }

        /// <summary>
        /// 设置加载更多事件
        /// </summary>
        /// <param name="res"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public View SetMore(int res, IOnLoadMoreListener listener)
        {
            var container = new FrameLayout(GetContext())
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent)
            };
            
            LayoutInflater.From(GetContext()).Inflate(res, container);
            GetEventDelegate().SetMore(container, listener);
            return container;
        }

        /// <summary>
        /// 设置加载更多
        /// </summary>
        /// <param name="view"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public View SetMore(View view, IOnLoadMoreListener listener)
        {
            GetEventDelegate().SetMore(view, listener);
            return view;
        }

        /// <summary>
        /// 设置取消加载更多
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public View SetNoMore(int res)
        {
            var container = new FrameLayout(GetContext())
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent)
            };
            LayoutInflater.From(GetContext()).Inflate(res, container);
            GetEventDelegate().SetNoMore(container);
            return container;
        }

        /// <summary>
        /// 设置取消加载更多
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public View SetNoMore(View view)
        {
            GetEventDelegate().SetNoMore(view);
            return view;
        }

        /// <summary>
        /// 设置错误更多
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public View SetError(int res)
        {
            var container = new FrameLayout(GetContext())
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent)
            };
            LayoutInflater.From(GetContext()).Inflate(res, container);
            GetEventDelegate().SetErrorMore(container);
            return container;
        }

        /// <summary>
        /// 设置错误更多
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public View SetError(View view)
        {
            GetEventDelegate().SetErrorMore(view);
            return view;
        }

        public override void RegisterAdapterDataObserver(RecyclerView.AdapterDataObserver observer)
        {
            if (observer is EasyRecyclerView.EasyDataObserver)
            {
                _observer = observer;
            }
            else
            {
                base.RegisterAdapterDataObserver(observer);
            }
        }


        /// <summary>
        /// 将指定的对象添加到数组的末尾。
        /// </summary>
        /// <param name="obj"></param>
        public void Add(T obj)
        {
            eventDelegate?.AddData(obj == null ? 0 : 1);
            
            if (obj != null)
            {
                lock (_lock)
                {
                    objects.Add(obj);
                }
            }

            _observer?.OnItemRangeInserted(GetCount() + 1, 1);
            
            if (_notifyOnChange) 
                NotifyItemInserted(headers.Count + GetCount() + 1);
            
            Log("add NotifyItemInserted " + (headers.Count + GetCount() + 1));
        }
        
        /// <summary>
        /// 将指定的Collection添加到数组的末尾。
        /// </summary>
        /// <param name="collection"></param>
        public void AddAll(ICollection<T> collection)
        {
            eventDelegate?.AddData(collection?.Count ?? 0);
            
            if (collection != null && collection.Count() != 0)
            {
                lock (_lock)
                {
                    objects.AddRange(collection);
                }
            }

            var dataCount = collection?.Count ?? 0;
            _observer?.OnItemRangeInserted(GetCount() - dataCount + 1, dataCount);
            
            if (_notifyOnChange)
                NotifyItemRangeInserted(headers.Count + GetCount() - dataCount + 1, dataCount);
            
            Log("addAll NotifyItemRangeInserted " + (headers.Count + GetCount() - dataCount + 1) + "," + dataCount);
        }


        /// <summary>
        /// 将指定的项目添加到数组的末尾。
        /// </summary>
        /// <param name="items"></param>
        public void AddAll(T[] items)
        {
            eventDelegate?.AddData(items?.Length ?? 0);
            
            if (items != null && items.Length != 0)
            {
                lock (_lock)
                {
                    objects.AddRange(items);
                }
            }

            var dataCount = items?.Length ?? 0;
            
            _observer?.OnItemRangeInserted(GetCount() - dataCount + 1, dataCount);
            
            if (_notifyOnChange)
                NotifyItemRangeInserted(headers.Count + GetCount() - dataCount + 1, dataCount);
            
            Log("addAll NotifyItemRangeInserted " + (headers.Count + GetCount() - dataCount + 1 + "," + dataCount));
        }


        /// <summary>
        /// 插入，不会触发任何事情
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="index"></param>
        public void Insert(T obj, int index)
        {
            lock (_lock)
            {
                objects.Insert(index, obj);
            }

            _observer?.OnItemRangeInserted(index, 1);
            if (_notifyOnChange) 
                NotifyItemInserted(headers.Count + index + 1);
            
            Log("insert NotifyItemRangeInserted " + (headers.Count + index + 1));
        }


        /// <summary>
        /// 插入数组，不会触发任何事情
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="index"></param>
        public void InsertAll(T[] objects, int index)
        {
            lock (_lock)
            {
                this.objects.InsertRange(index, objects);
            }

            var dataCount = objects.Length;
            _observer?.OnItemRangeInserted(index + 1, dataCount);
            
            if (_notifyOnChange) 
                NotifyItemRangeInserted(headers.Count + index + 1, dataCount);
            
            Log("insertAll NotifyItemRangeInserted " + (headers.Count + index + 1 + "," + dataCount));
        }


        /// <summary>
        /// 插入数组，不会触发任何事情
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="index"></param>
        public void InsertAll(ICollection<T> obj, int index)
        {
            lock (_lock)
            {
                objects.InsertRange(index, obj);
            }

            var dataCount = obj.Count;
            _observer?.OnItemRangeInserted(index + 1, dataCount);
            
            if (_notifyOnChange) 
                NotifyItemRangeInserted(headers.Count + index + 1, dataCount);
            
            Log("insertAll NotifyItemRangeInserted " + (headers.Count + index + 1 + "," + dataCount));
        }


        /// <summary>
        /// 删除，不会触发任何事情
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(T obj)
        {
            var position = objects.IndexOf(obj);
            lock (_lock)
            {
                if (!objects.Remove(obj)) 
                    return;
                
                _observer?.OnItemRangeRemoved(position, 1);
                    
                if (_notifyOnChange) 
                    NotifyItemRemoved(headers.Count + position);
                    
                Log("remove NotifyItemRemoved " + (headers.Count + position));
            }
        }


        /// <summary>
        /// 删除，不会触发任何事情
        /// </summary>
        /// <param name="position"></param>
        public void Remove(int position)
        {
            lock (_lock)
            {
                objects.RemoveAt(position);
            }

            _observer?.OnItemRangeRemoved(position, 1);
            
            if (_notifyOnChange) 
                NotifyItemRemoved(headers.Count + position);
            
            Log("remove NotifyItemRemoved " + (headers.Count + position));
        }


 
        /// <summary>
        /// 触发清空
        /// </summary>
        public void Clear()
        {
            var count = objects.Count;
            
            eventDelegate?.Clear();
            
            lock (_lock)
            {
                objects.Clear();
            }

            _observer?.OnItemRangeRemoved(0, count);
            
            if (_notifyOnChange) 
                NotifyItemRangeRemoved(headers.Count(), count);
            
            Log("clear NotifyItemRangeRemoved " + headers.Count + "," + count);
        }


        /// <summary>
        /// 使用指定的比较器对该适配器的内容进行排序。
        /// </summary>
        /// <param name="comparator"></param>
        public void Sort(IComparer<T> comparator)
        {
            lock (_lock)
            {
                objects.Sort(comparator);
            }

            if (_notifyOnChange)
                NotifyDataSetChanged();
        }


        public void SetNotifyOnChange(bool notifyOnChange)
        {
            _notifyOnChange = notifyOnChange;
        }



        /// <summary>
        /// 返回与此数组适配器关联的上下文。 使用上下文从传递给构造函数的资源创建视图。
        /// </summary>
        /// <returns></returns>
        public Context GetContext()
        {
            return _context;
        }

        /// <summary>
        /// 设置上下文
        /// </summary>
        /// <param name="ctx"></param>
        public void SetContext(Context ctx)
        {
            _context = ctx;
        }

   
        /// <summary>
        /// 这个函数包含了头部和尾部view的个数，不是真正的item个数。
        /// </summary>
        [Deprecated]
        public override int ItemCount => objects.Count + headers.Count + footers.Count;


        /// <summary>
        /// 应该使用这个获取item个数
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return objects.Count;
        }

        private View CreateSpViewByType(ViewGroup parent, int viewType)
        {
            foreach (var view in from headerView in headers where headerView.GetHashCode() == viewType select headerView.OnCreateView(parent))
            {
                StaggeredGridLayoutManager.LayoutParams layoutParams;
                if (view.LayoutParameters != null)
                    layoutParams = new StaggeredGridLayoutManager.LayoutParams(view.LayoutParameters);
                else
                    layoutParams = new StaggeredGridLayoutManager.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.WrapContent);
                layoutParams.FullSpan = true;
                view.LayoutParameters = layoutParams;
                return view;
            }

            foreach (var view in from footer in footers where footer.GetHashCode() == viewType select footer.OnCreateView(parent))
            {
                StaggeredGridLayoutManager.LayoutParams layoutParams;
                if (view.LayoutParameters != null)
                    layoutParams = new StaggeredGridLayoutManager.LayoutParams(view.LayoutParameters);
                else
                    layoutParams = new StaggeredGridLayoutManager.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.WrapContent);
                layoutParams.FullSpan = true;
                view.LayoutParameters = layoutParams;
                return view;
            }

            return null;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = CreateSpViewByType(parent, viewType);
            if (view != null)
            {
                return new StateViewHolder(view);
            }

            var viewHolder = onCreateViewHolder(parent, viewType);

            //itemView 的点击事件
            if (mItemClickListener != null)
            {
                viewHolder.ItemView.Click += (sender, e) =>
                {
                    mItemClickListener.OnItemClick(viewHolder.AdapterPosition - headers.Count);
                };
            }

            if (mItemLongClickListener != null)
            {
                viewHolder.ItemView.LongClick += (sender, e) =>
                {
                    e.Handled = mItemLongClickListener.OnItemLongClick(viewHolder.AdapterPosition - headers.Count);
                };
            }

            return viewHolder;
        }

        protected  abstract BaseViewHolder<T> onCreateViewHolder(ViewGroup parent, int viewType);

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (!(holder is BaseViewHolder<T> baseViewHolder)) 
                return;
            
            baseViewHolder.ItemView.Id = position;
            if (headers.Count != 0 && position < headers.Count)
            {
                headers[position].OnBindView(baseViewHolder.ItemView);
                return;
            }

            var i = position - headers.Count - objects.Count;
            if (footers.Count != 0 && i >= 0)
            {
                footers[i].OnBindView(baseViewHolder.ItemView);
                return;
            }

            OnBindViewHolder(baseViewHolder, position - headers.Count);
        }

        public void OnBindViewHolder(BaseViewHolder<T> holder, int position)
        {
            holder.SetData(GetItem(position));
        }

        [Deprecated]
        public override int GetItemViewType(int position)
        {
            if (headers.Count() != 0)
            {
                if (position < headers.Count)
                    return headers[position].GetHashCode();
            }

            if (footers.Count == 0) 
                return GetViewType(position - headers.Count);
            
            var i = position - headers.Count - objects.Count;
            
            return i >= 0 ? footers[i].GetHashCode() : GetViewType(position - headers.Count());
        }

        public int GetViewType(int position)
        {
            return 0;
        }

        public List<T> GetAllData()
        {
            return new List<T>(objects);
        }
        
        public T GetItem(int position)
        {
            return objects[position];
        }

        /// <summary>
        /// 返回指定项目在数组中的位置。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetPosition(T item)
        {
            return objects.IndexOf(item);
        }

        public new long GetItemId(int position)
        {
            return position;
        }

        private class StateViewHolder : BaseViewHolder<T>
        {
            public StateViewHolder(View itemView)
                : base(itemView)
            {
            }
        }

        public interface IOnItemClickListener
        {
            void OnItemClick(int position);
        }

        public interface IOnItemLongClickListener
        {
            bool OnItemLongClick(int position);
        }

        public void SetOnItemClickListener(IOnItemClickListener listener)
        {
            mItemClickListener = listener;
        }

        public void SetOnItemLongClickListener(IOnItemLongClickListener listener)
        {
            mItemLongClickListener = listener;
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