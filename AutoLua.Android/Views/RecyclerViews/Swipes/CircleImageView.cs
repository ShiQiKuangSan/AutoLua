using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Views.Animations;
using Android.Widget;

namespace AutoLua.Droid.Views.RecyclerViews.Swipes
{
    /// <summary>
    /// 圆形图像视图
    /// </summary>
    [Register("AutoLua.Droid.Views.RecyclerViews.Swipes.CircleImageView")]
    public sealed class CircleImageView : ImageView
    {
        private const int KeyShadowColor = 0x1E000000;

        private const int FillShadowColor = 0x3D000000;

        // PX
        private const float XOffset = 0f;
        private const float YOffset = 1.75f;
        private const float ShadowRadius = 3.5f;
        private const int ShadowElevation = 4;

        /// <summary>
        /// 动画监听
        /// </summary>
        private Animation.IAnimationListener _listener;
        
        private int _shadowRadius;

        public CircleImageView(Context context, int color, float radius) : base(context)
        {

            var density = Context.Resources.DisplayMetrics.Density;
            var diameter = (int)(radius * density * 2);
            var shadowYOffset = (int)(density * YOffset);
            var shadowXOffset = (int)(density * XOffset);

            _shadowRadius = (int)(density * ShadowRadius);

            ShapeDrawable circle;
            if (ElevationSupported())
            {
                circle = new ShapeDrawable(new OvalShape());
                ViewCompat.SetElevation(this, ShadowElevation * density);
            }
            else
            {
                OvalShape oval = new OvalShadow(this, _shadowRadius, diameter);
                circle = new ShapeDrawable(oval);
#pragma warning disable 618
                ViewCompat.SetLayerType(this, ViewCompat.LayerTypeSoftware, circle.Paint);
#pragma warning restore 618
                circle.Paint.SetShadowLayer(_shadowRadius, shadowXOffset, shadowYOffset,
                        new Color(KeyShadowColor));
                var padding = _shadowRadius;
                SetPadding(padding, padding, padding, padding);
            }
            circle.Paint.Color = new Color(color);
#pragma warning disable 618
            SetBackgroundDrawable(circle);
#pragma warning restore 618
        }

        private static bool ElevationSupported()
        {
            return Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            if (!ElevationSupported())
            {
                SetMeasuredDimension(MeasuredWidth + _shadowRadius * 2, MeasuredHeight
                        + _shadowRadius * 2);
            }
        }

        public void SetAnimationListener(Animation.IAnimationListener listener)
        {
            _listener = listener;
        }

        protected override void OnAnimationStart()
        {
            base.OnAnimationStart();
            _listener?.OnAnimationStart(Animation);
        }

        protected override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
            _listener?.OnAnimationEnd(Animation);
        }
        
        /// <summary>
        /// 更新圆形图像视图的背景色。
        /// </summary>
        /// <param name="colorRes"></param>
        public void SetBackgroundColorRes(int colorRes)
        {
            SetBackgroundColor(ContextCompat.GetColor(Context, colorRes));
        }

        public override void SetBackgroundColor(Color color)
        {
            if (Background is ShapeDrawable drawable)
            {
                drawable.Paint.Color = color;
            }
        }

        public void SetBackgroundColor(int color)
        {
            if (Background is ShapeDrawable drawable)
            {
                drawable.Paint.Color = new Color(color);
            }
        }

        private class OvalShadow : OvalShape
        {

            private readonly CircleImageView _circleImageView;

            private readonly Paint _shadowPaint;
            private readonly int _circleDiameter;

            public OvalShadow(CircleImageView circleImageView, int shadowRadius, int circleDiameter) : base()
            {
                _circleImageView = circleImageView;
                _shadowPaint = new Paint();
                _circleImageView._shadowRadius = shadowRadius;
                _circleDiameter = circleDiameter;
                var radialGradient = new RadialGradient(_circleDiameter / 2, _circleDiameter / 2,
                    _circleImageView._shadowRadius, new int[] {
                        FillShadowColor, Color.Transparent.ToArgb()
                    }, null, Shader.TileMode.Clamp);
                
                _shadowPaint.SetShader(radialGradient);
            }

            public override void Draw(Canvas canvas, Paint paint)
            {
                var viewWidth = _circleImageView.Width;
                var viewHeight = _circleImageView.Height;
                canvas.DrawCircle(viewWidth / 2, viewHeight / 2, (_circleDiameter / 2 + _circleImageView._shadowRadius),
                        _shadowPaint);
                canvas.DrawCircle(viewWidth / 2, viewHeight / 2, (_circleDiameter / 2), paint);
            }
        }
    }
}