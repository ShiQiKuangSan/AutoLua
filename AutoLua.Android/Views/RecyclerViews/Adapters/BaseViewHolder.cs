using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AutoLua.Core.Glide;
using Com.Bumptech.Glide;

namespace AutoLua.Droid.Views.RecyclerViews.Adapters
{
    public abstract class BaseViewHolder<T> : RecyclerView.ViewHolder where T : class
    {
        protected BaseViewHolder<T> Holder;

        private readonly int _layoutId;

        private Context _context;

        /// <summary>
        /// 转换视图
        /// </summary>
        private readonly View _convertView;

        private readonly SparseArray<View> _views = new SparseArray<View>();

        protected BaseViewHolder(View itemView) : base(itemView)
        {
            Holder = this;
            _convertView = itemView;
            _context = _convertView.Context;
        }

        protected BaseViewHolder(ViewGroup parent, int res)
            : base(LayoutInflater.From(parent.Context).Inflate(res, parent, false))
        {
            Holder = this;
            _convertView = ItemView;
            _layoutId = res;
            _context = _convertView.Context;
        }

        public virtual void SetData(T item)
        {
        }

        /// <summary>
        /// 上下文
        /// </summary>
        public Context Context => _context ??= ItemView.Context;

        /// <summary>
        /// 获得视图
        /// </summary>
        /// <param name="viewId">控件id</param>
        /// <returns></returns>
        public View GetView(int viewId)
        {
            return GetView<View>(viewId);
        }

        /// <summary>
        /// 获得视图
        /// </summary>
        /// <param name="viewId">控件id</param>
        /// <typeparam name="TV">控件类型</typeparam>
        /// <returns></returns>
        public TV GetView<TV>(int viewId) where TV : View
        {
            var view = _views.Get(viewId);

            if (view != null)
                return (TV) view;

            view = _convertView.FindViewById(viewId);
            _views.Put(viewId, view);

            return (TV) view;
        }

        /// <summary>
        /// 获得布局
        /// </summary>
        public int LayoutId => _layoutId;

        /// <summary>
        /// 获得item布局
        /// </summary>
        /// <returns></returns>
        public View GetItemView()
        {
            return _convertView;
        }

        /// <summary>
        /// 设置子项视图的点击事件
        /// </summary>
        /// <param name="click"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetOnItemViewClick(Action<object, EventArgs> click)
        {
            if (click != null)
            {
                _convertView.Click += (sender, args) => click(sender, args);
            }

            return this;
        }

        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetText(int viewId, string value)
        {
            var view = GetView<TextView>(viewId);
            view.Text = value;
            return this;
        }

        /// <summary>
        /// 设置文本颜色
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetTextColor(int viewId, int color)
        {
            var view = GetView<TextView>(viewId);
            view.SetTextColor(new Color(color));
            return this;
        }

        /// <summary>
        /// 设置文本颜色资源
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="colorRes"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetTextColorRes(int viewId, int colorRes)
        {
            var view = GetView<TextView>(viewId);
            view.SetTextColor(new Color(ContextCompat.GetColor(Context, colorRes)));
            return this;
        }

        /// <summary>
        /// 设置图片资源
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="imgResId"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetImageResource(int viewId, int imgResId)
        {
            var view = GetView<ImageView>(viewId);
            view.SetImageResource(imgResId);
            return this;
        }

        /// <summary>
        /// 设置背景颜色
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetBackgroundColor(int viewId, int color)
        {
            var view = GetView(viewId);
            view.SetBackgroundColor(new Color(color));
            return this;
        }

        /// <summary>
        /// 设置背景颜色资源
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="colorRes"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetBackgroundColorRes(int viewId, int colorRes)
        {
            var view = GetView(viewId);
            view.SetBackgroundResource(colorRes);
            return this;
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="drawable"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetImageDrawable(int viewId, Drawable drawable)
        {
            var view = GetView<ImageView>(viewId);
            view.SetImageDrawable(drawable);
            return this;
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="drawableRes"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetImageDrawableRes(int viewId, int drawableRes)
        {
            var drawable = ContextCompat.GetDrawable(Context, drawableRes);
            return SetImageDrawable(viewId, drawable);
        }

        /// <summary>
        /// 设置图片url地址
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="imgUrl"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetImageUrl(int viewId, string imgUrl)
        {
            var view = GetView<ImageView>(viewId);
            Glide.With(Context).Load(imgUrl).Into(view);
            return this;
        }

        /// <summary>
        /// 设置图片url地址
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="imgUrl"></param>
        /// <param name="placeHolderRes"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetImageUrl(int viewId, string imgUrl, int placeHolderRes)
        {
            var view = GetView<ImageView>(viewId);
            Glide.With(Context).Load(imgUrl)
                .Placeholder(placeHolderRes)
                .Into(view);
            return this;
        }

        /// <summary>
        /// 设置图片url地址
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="imgUrl"></param>
        /// <param name="placeHolderRes"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetCircleImageUrl(int viewId, string imgUrl, int placeHolderRes)
        {
            var view = GetView<ImageView>(viewId);
            Glide.With(Context).Load(imgUrl)
                .Placeholder(placeHolderRes)
                .Transform(new GlideRoundTransform(Context))
                .Into(view);
            return this;
        }

        /// <summary>
        /// 设置圆形图片url地址
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="imgUrl"></param>
        /// <param name="placeHolderRes"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetRoundImageUrl(int viewId, string imgUrl, int placeHolderRes)
        {
            var view = GetView<ImageView>(viewId);
            Glide.With(Context).Load(imgUrl)
                .Placeholder(placeHolderRes)
                .Transform(new GlideRoundTransform(Context))
                .Into(view);
            return this;
        }

        /// <summary>
        /// 设置图片Bitmap
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="imgBitmap"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetImageBitmap(int viewId, Bitmap imgBitmap)
        {
            var view = GetView<ImageView>(viewId);
            view.SetImageBitmap(imgBitmap);
            return this;
        }

        /// <summary>
        /// 设置视图是否可见
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetVisible(int viewId, bool visible)
        {
            var view = GetView(viewId);
            view.Visibility = visible ? ViewStates.Visible : ViewStates.Gone;
            return this;
        }

        /// <summary>
        /// 设置可见
        /// </summary>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetVisible(int viewId)
        {
            var view = GetView(viewId);
            view.Visibility = ViewStates.Visible;
            return this;
        }

        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetTag(int viewId, Java.Lang.Object tag)
        {
            var view = GetView(viewId);
            view.Tag = tag;
            return this;
        }

        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="key"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetTag(int viewId, int key, Java.Lang.Object tag)
        {
            var view = GetView(viewId);
            view.SetTag(key, tag);
            return this;
        }

        /// <summary>
        /// 设置是否勾选
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetChecked(int viewId, bool check)
        {
            if (GetView(viewId) is ICheckable view)
                view.Checked = check;

            return this;
        }

        /// <summary>
        /// 设置法线动画
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetAlpha(int viewId, float value)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                GetView<View>(viewId).Alpha = value;
            }
            else
            {
                var alpha = new AlphaAnimation(value, value)
                {
                    Duration = 0,
                    FillAfter = true
                };

                GetView<View>(viewId).StartAnimation(alpha);
            }

            return this;
        }

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="typeface"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetTypeface(int viewId, Typeface typeface)
        {
            var view = GetView<TextView>(viewId);
            view.Typeface = typeface;
            view.PaintFlags |= PaintFlags.SubpixelText;
            return this;
        }

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="typeface"></param>
        /// <param name="viewIds"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetTypeface(Typeface typeface, IEnumerable<int> viewIds)
        {
            foreach (var viewId in viewIds)
            {
                var view = GetView<TextView>(viewId);
                view.Typeface = typeface;
                view.PaintFlags |= PaintFlags.SubpixelText;
            }

            return this;
        }

        /// <summary>
        /// 设置点击事件
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="click"></param>
        /// <returns></returns>
        public BaseViewHolder<T> SetOnClick(int viewId, Action<object, EventArgs> click)
        {
            var view = GetView(viewId);

            if (click != null && view != null)
            {
                view.Click += (sender, args) => click(sender, args);
            }

            return this;
        }
    }
}