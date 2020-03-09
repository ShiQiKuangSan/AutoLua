using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Views;
using AutoLua.Droid.Views.RecyclerViews.Adapters;

namespace AutoLua.Droid.Views.RecyclerViews.Decorations
{
    public class DividerDecoration : RecyclerView.ItemDecoration
    {
        private readonly ColorDrawable _colorDrawable;
        private readonly int _height;
        private readonly int _paddingLeft;
        private readonly int _paddingRight;
        private bool _drawLastItem = true;
        private bool _drawHeaderFooter = false;

        public DividerDecoration(int color, int height)
        {
            _colorDrawable = new ColorDrawable(new Color(color));
            _height = height;
        }

        public DividerDecoration(int color, int height, int paddingLeft, int paddingRight)
        {
            _colorDrawable = new ColorDrawable(new Color(color));
            _height = height;
            _paddingLeft = paddingLeft;
            _paddingRight = paddingRight;
        }

        public void SetDrawLastItem(bool mDrawLastItem)
        {
            _drawLastItem = mDrawLastItem;
        }

        public void SetDrawHeaderFooter(bool mDrawHeaderFooter)
        {
            _drawHeaderFooter = mDrawHeaderFooter;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            var position = parent.GetChildAdapterPosition(view);
            var orientation = 0;
            int headerCount = 0, footerCount = 0;
            var adapterObj = parent.GetAdapter();
            var adapterType = adapterObj.GetType();
            if (adapterType.IsAssignableFrom(typeof(RecyclerArrayAdapter<>)))
            {
                // ReSharper disable once PossibleNullReferenceException
                headerCount = (int) adapterType.GetMethod("GetHeaderCount")?.Invoke(adapterObj, null);
                // ReSharper disable once PossibleNullReferenceException
                footerCount = (int) adapterType.GetMethod("GetFooterCount")?.Invoke(adapterObj, null);
            }

            var layoutManager = parent.GetLayoutManager();
            switch (layoutManager)
            {
                case StaggeredGridLayoutManager manager:
                    orientation = manager.Orientation;
                    break;
                case GridLayoutManager gridLayoutManager:
                    orientation = gridLayoutManager.Orientation;
                    break;
                case LinearLayoutManager linearLayoutManager:
                    orientation = linearLayoutManager.Orientation;
                    break;
            }

            if ((position < headerCount || position >= parent.GetAdapter().ItemCount - footerCount) &&
                !_drawHeaderFooter) 
                return;
            
            if (orientation == OrientationHelper.Vertical)
            {
                outRect.Bottom = _height;
            }
            else
            {
                outRect.Right = _height;
            }
        }

        public void onDrawOver(Canvas c, RecyclerView parent, RecyclerView.State state)
        {
            var orientation = 0;
            int headerCount = 0, footerCount = 0, dataCount;
            var adapterObj = parent.GetAdapter();
            var adapterType = adapterObj.GetType();
            if (adapterType.IsAssignableFrom(typeof(RecyclerArrayAdapter<>)))
            {
                headerCount = (int) adapterType.GetMethod("GetHeaderCount").Invoke(adapterObj, null);
                footerCount = (int) adapterType.GetMethod("GetFooterCount").Invoke(adapterObj, null);
                dataCount = (int) adapterType.GetMethod("GetCount").Invoke(adapterObj, null);
            }
            else
            {
                dataCount = parent.GetAdapter().ItemCount;
            }

            var dataStartPosition = headerCount;
            var dataEndPosition = headerCount + dataCount;


            var layoutManager = parent.GetLayoutManager();
            
            switch (layoutManager)
            {
                case StaggeredGridLayoutManager manager:
                    orientation = manager.Orientation;
                    break;
                case GridLayoutManager gridLayoutManager:
                    orientation = gridLayoutManager.Orientation;
                    break;
                case LinearLayoutManager linearLayoutManager:
                    orientation = linearLayoutManager.Orientation;
                    break;
            }

            int start, end;
            if (orientation == OrientationHelper.Vertical)
            {
                start = parent.PaddingLeft + _paddingLeft;
                end = parent.Width - parent.PaddingRight - _paddingRight;
            }
            else
            {
                start = parent.PaddingTop + _paddingLeft;
                end = parent.Height - parent.PaddingBottom - _paddingRight;
            }

            var childCount = parent.ChildCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = parent.GetChildAt(i);
                var position = parent.GetChildAdapterPosition(child);

                if ((position < dataStartPosition || position >= dataEndPosition - 1) &&
                    (position != dataEndPosition - 1 || !_drawLastItem) &&
                    (position >= dataStartPosition && position < dataEndPosition || !_drawHeaderFooter)) 
                    continue;
                
                if (orientation == OrientationHelper.Vertical)
                {
                    var layoutParams = (RecyclerView.LayoutParams) child.LayoutParameters;
                    var top = child.Bottom + layoutParams.BottomMargin;
                    var bottom = top + _height;
                    _colorDrawable.SetBounds(start, top, end, bottom);
                    _colorDrawable.Draw(c);
                }
                else
                {
                    var layoutParams = (RecyclerView.LayoutParams) child.LayoutParameters;
                    var left = child.Right + layoutParams.RightMargin;
                    var right = left + _height;
                    _colorDrawable.SetBounds(left, start, right, end);
                    _colorDrawable.Draw(c);
                }
            }
        }
    }
}