using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Support.V4.View;
using Android.Graphics.Drawables;
using Android.Util;
using Java.Lang;
using static Android.Graphics.Paint;
using Math = System.Math;

namespace AutoLua.Droid.Views
{
    /// <summary>
    /// 指示器。
    /// </summary>
    [Register("AutoLua.Droid.Views.RVPIndicator")]
    public class RVPIndicator : LinearLayout
    {
        // 指示器风格-图标
        private const int StyleBitmap = 0;

        // 指示器风格-下划线
        private const int StyleLine = 1;

        // 指示器风格-方形背景
        private const int StyleSquare = 2;

        // 指示器风格-三角形
        private const int StyleTriangle = 3;

        /*
         * 系统默认:Tab数量
         */
        private const int DefaultTabCount = 3;

        /*
         * 系统默认:文字正常时颜色
         */
        private static readonly Color DefaultTextColorNormal = Color.ParseColor("#000000");

        /*
         * 系统默认:文字选中时颜色
         */
        private static readonly Color DefaultTextColorHighlight = Color.ParseColor("#FF0000");

        /*
         * 系统默认:指示器颜色
         */
        private static readonly Color DefaultIndicatorColor = Color.ParseColor("#f29b76");

        /**
         * tab上的内容
         */
        private List<string> _tabTitles;

        /**
         * 可见tab数量
         */
        private readonly int _tabVisibleCount = DefaultTabCount;

        /**
         * 与之绑定的ViewPager
         */
        public ViewPager ViewPager;

        /**
         * 文字大小
         */
        private readonly int _textSize = 16;

        /**
         * 文字正常时的颜色
         */
        private readonly Color _textColorNormal = DefaultTextColorNormal;

        /**
         * 文字选中时的颜色
         */
        private readonly Color _textColorHighlight = DefaultTextColorHighlight;

        /**
         * 画笔
         */
        private readonly Paint _paint;

        /**
         * 矩形
         */
        private Rect _rectF;

        /**
         * bitmap
         */
        private readonly Bitmap _bitmap;

        /**
         * 指示器高
         */
        private int _indicatorHeight = 5;

        /**
         * 指示器宽
         */
        private int _indicatorWidth;

        /**
         * 三角形的宽度为单个Tab的1/6
         */
        private static readonly float RADIO_TRIANGEL = 1.0f / 6;

        /**
         * 手指滑动时的偏移量
         */
        private float _translationX;

        /**
         * 指示器风格
         */
        private readonly int _indicatorStyle = StyleLine;

        /**
         * 曲线path
         */
        private Path _path;

        /**
         * viewPage初始下标
         */
        private int _position = 0;

        /// <summary>
        /// 选中页面
        /// </summary>
        public event EventHandler<ViewPager.PageSelectedEventArgs> OnPageSelected;

        /// <summary>
        /// 滚动页面
        /// </summary>
        public event EventHandler<ViewPager.PageScrolledEventArgs> OnPageScrolled;

        /// <summary>
        /// 页面滚动状态改变
        /// </summary>
        public event EventHandler<ViewPager.PageScrollStateChangedEventArgs> OnPageScrollStateChanged;

        public RVPIndicator(Context context, IAttributeSet attrs = null) : base(context, attrs)
        {
            _indicatorWidth = Width / _tabVisibleCount;
            // 获得自定义属性
            var a = context.ObtainStyledAttributes(attrs,Resource.Styleable.RVPIndicator);

            _tabVisibleCount = a.GetInt(Resource.Styleable.RVPIndicator_item_count, DefaultTabCount);
            _textColorNormal = a.GetColor(Resource.Styleable.RVPIndicator_text_color_normal,DefaultTextColorNormal);
            _textColorHighlight = a.GetColor(Resource.Styleable.RVPIndicator_text_color_hightlight,DefaultTextColorHighlight);
            _textSize = a.GetDimensionPixelSize(Resource.Styleable.RVPIndicator_text_size,16);
            var indicatorColor = a.GetColor(Resource.Styleable.RVPIndicator_indicator_color,DefaultIndicatorColor);
            _indicatorStyle = a.GetInt(Resource.Styleable.RVPIndicator_indicator_style,StyleLine);

            var drawable = a.GetDrawable(Resource.Styleable.RVPIndicator_indicator_src);

            if (drawable != null)
            {
                switch (drawable)
                {
                    case BitmapDrawable bitmapDrawable:
                        _bitmap = bitmapDrawable.Bitmap;
                        break;
                    case NinePatchDrawable _:
                    {
                        // .9图处理
                        var bitmap = Bitmap.CreateBitmap(
                            drawable.IntrinsicWidth,
                            drawable.IntrinsicHeight, Bitmap.Config.Argb8888);
                        var canvas = new Canvas(bitmap);
                        drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
                        drawable.Draw(canvas);
                        _bitmap = bitmap;
                        break;
                    }
                }
            }

            a.Recycle();

            _paint = new Paint
            {
                AntiAlias = true, 
                Color = new Color(indicatorColor)
            };
            _paint.SetStyle(Style.Fill);
        }

        /// <summary>
        /// 初始化指示器
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="oldw"></param>
        /// <param name="oldh"></param>
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            switch (_indicatorStyle)
            {
                case StyleLine:

                    /*
                     * 下划线指示器:宽与item相等,高是item的1/10
                     */
                    _indicatorWidth = w / _tabVisibleCount;
                    _indicatorHeight = h / 10;
                    _translationX = 0;
                    _rectF = new Rect(0, 0, _indicatorWidth, _indicatorHeight);

                    break;
                case StyleSquare:
                case StyleBitmap:

                    /*
                     * 方形/图标指示器:宽,高与item相等
                     */
                    _indicatorWidth = w / _tabVisibleCount;
                    _indicatorHeight = h;
                    _translationX = 0;
                    _rectF = new Rect(0, 0, _indicatorWidth, _indicatorHeight);

                    break;
                case StyleTriangle:

                    /*
                     * 三角形指示器:宽与item(1/4)相等,高是item的1/4
                     */
                    //mIndicatorWidth = w / mTabVisibleCount / 4;
                    // mIndicatorHeight = h / 4;
                    _indicatorWidth = (int) (w / _tabVisibleCount * RADIO_TRIANGEL); // 1/6 of  width  ;
                    _indicatorHeight = (int) (_indicatorWidth / 2 / Math.Sqrt(2));
                    _translationX = 0;

                    break;
            }

            // 初始化tabItem
            InitTabItem();

            // 高亮
            SetHighLightTextView(_position);
        }

        /**
         * 绘制指示器(子view)
         */
        protected override void DispatchDraw(Canvas canvas)
        {
            // 保存画布
            canvas.Save();

            switch (_indicatorStyle)
            {
                case StyleBitmap:

                    canvas.Translate(_translationX, 0);
                    canvas.DrawBitmap(_bitmap, null, _rectF, _paint);

                    break;
                case StyleLine:

                    canvas.Translate(_translationX, Height - _indicatorHeight);
                    canvas.DrawRect(_rectF, _paint);

                    break;
                case StyleSquare:

                    canvas.Translate(_translationX, 0);
                    canvas.DrawRect(_rectF, _paint);

                    break;
                case StyleTriangle:

                    canvas.Translate(_translationX, 0);
                    // 笔锋圆滑度
                    // mPaint.setPathEffect(new CornerPathEffect(10));
                    _path = new Path();
                    var midOfTab = Width / _tabVisibleCount / 2;
                    _path.MoveTo(midOfTab, Height - _indicatorHeight);
                    _path.LineTo(midOfTab - _indicatorWidth / 2, Height);
                    _path.LineTo(midOfTab + _indicatorWidth / 2, Height);
                    _path.Close();
                    canvas.DrawPath(_path, _paint);

                    break;
            }

            // 恢复画布
            canvas.Restore();
            base.DispatchDraw(canvas);
        }

        /// <summary>
        /// 设置tab上的内容
        /// </summary>
        /// <param name="titles"></param>
        public void SetTabItemTitles(List<string> titles)
        {
            _tabTitles = titles;
        }

        /// <summary>
        /// 设置关联viewPager
        /// </summary>
        /// <param name="viewPager"></param>
        /// <param name="pos"></param>
        [SuppressWarnings(Value = new[] {"deprecation"})]
        public void SetViewPager(ViewPager viewPager, int pos)
        {
            ViewPager = viewPager;

            //页面选中
            ViewPager.PageSelected += (sender, args) =>
            {
                SetHighLightTextView(args.Position);

                //回调给外部
                OnPageSelected?.Invoke(this, new ViewPager.PageSelectedEventArgs(args.Position));
            };
            //页面滚动
            ViewPager.PageScrolled += (sender, args) =>
            {
                OnScoll(args.Position, args.PositionOffset);

                OnPageScrolled?.Invoke(this,
                    new ViewPager.PageScrolledEventArgs(args.Position, args.PositionOffset, args.PositionOffsetPixels));
            };

            //页面滚动状态
            ViewPager.PageScrollStateChanged += (sender, args) =>
            {
                OnPageScrollStateChanged?.Invoke(this, new ViewPager.PageScrollStateChangedEventArgs(args.State));
            };

            // 设置当前页
            ViewPager.CurrentItem = pos;
            _position = pos;
        }

        /// <summary>
        /// 初始化tabItem
        /// </summary>
        private void InitTabItem()
        {
            if (_tabTitles != null && _tabTitles.Count > 0)
            {
                Post(() =>
                {
                    if (ChildCount != 0)
                    {
                        RemoveAllViews();
                    }

                    foreach (var title in _tabTitles)
                    {
                        AddView(CreateTextView(title));
                    }

                    // 设置点击事件
                    SetItemClickEvent();
                });
            }
        }

        /// <summary>
        /// 设置点击事件
        /// </summary>
        private void SetItemClickEvent()
        {
            var cCount = ChildCount;
            for (var i = 0; i < cCount; i++)
            {
                var j = i;
                var view = GetChildAt(i);
                view.Click += (sender, e) => { ViewPager.CurrentItem = j; };
            }
        }

        /// <summary>
        /// 设置文本高亮
        /// </summary>
        /// <param name="position"></param>
        private void SetHighLightTextView(int position)
        {
            Post(() =>
            {
                for (var i = 0; i < ChildCount; i++)
                {
                    var view = GetChildAt(i);
                    if (view is TextView textView)
                    {
                        textView.SetTextColor(i == position ? _textColorHighlight : _textColorNormal);
                    }
                }
            });
        }

        /// <summary>
        /// 滚动
        /// </summary>
        /// <param name="position"></param>
        /// <param name="offset"></param>
        private void OnScoll(int position, float offset)
        {
            // 不断改变偏移量，invalidate
            _translationX = Width / _tabVisibleCount * (position + offset);

            var tabWidth = Width / _tabVisibleCount;

            // 容器滚动，当移动到倒数第二个的时候，开始滚动
            if (offset > 0 && position >= (_tabVisibleCount - 2)
                           && ChildCount > _tabVisibleCount
                           && position < (ChildCount - 2))
            {
                if (_tabVisibleCount != 1)
                {
                    var xValue = (position - (_tabVisibleCount - 2)) * tabWidth
                                 + (int) (tabWidth * offset);
                    ScrollTo(xValue, 0);
                }
                else
                {
                    // 为count为1时 的特殊处理
                    this.ScrollTo(position * tabWidth + (int) (tabWidth * offset),
                        0);
                }
            }

            Invalidate();
        }

        /// <summary>
        /// 创建标题view
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private TextView CreateTextView(string text)
        {
            var tv = new TextView(Context);
            var lp = new LayoutParams(
                ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            {
                Width = Width / _tabVisibleCount
            };
            tv.Gravity = (GravityFlags.Center);
            tv.SetTextColor(_textColorNormal);
            tv.Text = text;
            tv.SetTextSize(ComplexUnitType.Sp, _textSize);
            tv.LayoutParameters = lp;
            return tv;
        }
    }
}