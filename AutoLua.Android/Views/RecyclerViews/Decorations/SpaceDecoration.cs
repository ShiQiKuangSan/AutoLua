using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using AutoLua.Droid.Views.RecyclerViews.Adapters;

namespace AutoLua.Droid.Views.RecyclerViews.Decorations
{
    public class SpaceDecoration : RecyclerView.ItemDecoration
    {
        private readonly int _halfSpace;
        private bool _paddingEdgeSide = true;
        private bool _paddingStart = true;
        private bool _paddingHeaderFooter;


        public SpaceDecoration(int space)
        {
            _halfSpace = space / 2;
        }

        public void SetPaddingEdgeSide(bool mPaddingEdgeSide)
        {
            _paddingEdgeSide = mPaddingEdgeSide;
        }

        public void SetPaddingStart(bool mPaddingStart)
        {
            _paddingStart = mPaddingStart;
        }

        public void SetPaddingHeaderFooter(bool mPaddingHeaderFooter)
        {
            _paddingHeaderFooter = mPaddingHeaderFooter;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            var position = parent.GetChildAdapterPosition(view);
            var spanCount = 0;
            var orientation = 0;
            var spanIndex = 0;
            int headerCount = 0, footerCount = 0;

            var adapterObj = parent.GetAdapter();
            var adapterType = adapterObj.GetType();
            if (adapterType.IsAssignableFrom(typeof(RecyclerArrayAdapter<>)))
            {
                headerCount = (int)adapterType.GetMethod("GetHeaderCount").Invoke(adapterObj, null);
                footerCount = (int)adapterType.GetMethod("GetFooterCount").Invoke(adapterObj, null);
            }

            var layoutManager = parent.GetLayoutManager();
            
            switch (layoutManager)
            {
                case StaggeredGridLayoutManager manager:
                    orientation = manager.Orientation;
                    spanCount = manager.SpanCount;
                    spanIndex = ((StaggeredGridLayoutManager.LayoutParams)view.LayoutParameters).SpanIndex;
                    break;
                case GridLayoutManager gridLayoutManager:
                    orientation = gridLayoutManager.Orientation;
                    spanCount = gridLayoutManager.SpanCount;
                    spanIndex = ((GridLayoutManager.LayoutParams)view.LayoutParameters).SpanIndex;
                    break;
                case LinearLayoutManager linearLayoutManager:
                    orientation = linearLayoutManager.Orientation;
                    spanCount = 1;
                    spanIndex = 0;
                    break;
            }
 
            //普通Item的尺寸
            if ((position >= headerCount && position < parent.GetAdapter().ItemCount - footerCount))
            {
                GravityFlags gravity;
                if (spanIndex == 0 && spanCount > 1) gravity = GravityFlags.Left;
                else if (spanIndex == spanCount - 1 && spanCount > 1) gravity = GravityFlags.Right;
                else if (spanCount == 1) gravity = GravityFlags.FillHorizontal;
                else
                {
                    gravity = GravityFlags.Center;
                }
                if (orientation == OrientationHelper.Vertical)
                {
                    switch (gravity)
                    {
                        case GravityFlags.Left:
                            if (_paddingEdgeSide)
                                outRect.Left = _halfSpace * 2;
                            outRect.Right = _halfSpace;
                            break;
                        case GravityFlags.Right:
                            outRect.Left = _halfSpace;
                            if (_paddingEdgeSide)
                                outRect.Right = _halfSpace * 2;
                            break;
                        case GravityFlags.FillHorizontal:
                            if (_paddingEdgeSide)
                            {
                                outRect.Left = _halfSpace * 2;
                                outRect.Right = _halfSpace * 2;
                            }
                            break;
                        case GravityFlags.Center:
                            outRect.Left = _halfSpace;
                            outRect.Right = _halfSpace;
                            break;
                    }
                    if (position - headerCount < spanCount && _paddingStart) outRect.Top = _halfSpace * 2;
                    outRect.Bottom = _halfSpace * 2;
                }
                else
                {
                    switch (gravity)
                    {
                        case GravityFlags.Left:
                            if (_paddingEdgeSide)
                                outRect.Bottom = _halfSpace * 2;
                            outRect.Top = _halfSpace;
                            break;
                        case GravityFlags.Right:
                            outRect.Bottom = _halfSpace;
                            if (_paddingEdgeSide)
                                outRect.Top = _halfSpace * 2;
                            break;
                        case GravityFlags.FillHorizontal:
                            if (_paddingEdgeSide)
                            {
                                outRect.Left = _halfSpace * 2;
                                outRect.Right = _halfSpace * 2;
                            }
                            break;
                        case GravityFlags.Center:
                            outRect.Bottom = _halfSpace;
                            outRect.Top = _halfSpace;
                            break;
                    }
                    if (position - headerCount < spanCount && _paddingStart) outRect.Left = _halfSpace * 2;
                    outRect.Right = _halfSpace * 2;
                }
            }
            else
            {
                //只有HeaderFooter进到这里
                if (!_paddingHeaderFooter) 
                    return;
                
                //并且需要padding Header&Footer
                if (orientation == OrientationHelper.Vertical)
                {
                    if (_paddingEdgeSide)
                    {
                        outRect.Left = _halfSpace * 2;
                        outRect.Right = _halfSpace * 2;
                    }
                    outRect.Top = _halfSpace * 2;
                }
                else
                {
                    if (_paddingEdgeSide)
                    {
                        outRect.Top = _halfSpace * 2;
                        outRect.Bottom = _halfSpace * 2;
                    }
                    outRect.Left = _halfSpace * 2;
                }
            }
        }
    }
}