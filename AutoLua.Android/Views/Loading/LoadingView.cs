using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Util;
using Android.Graphics;
using Android.Animation;
using Java.Lang;
using Android.Support.V4.View.Animation;
using Android.Views.Animations;
using static Android.Animation.Animator;


namespace AutoLua.Droid.Views.Loading
{
    [Register("AutoLua.Droid.Views.Loading.LoadingView")]
    public class LoadingView : View
    {
        //the size in wrap_content model
        private const int CircleDiameter = 56;

        private const float CenterRadius = 15f;
        private const float StrokeWidth = 3.5f;

        private const float MaxProgressArc = 300f;
        private const float MinProgressArc = 20f;

        private const long AnimatorDuration = 1332;

        private Rect _bounds;
        private Ring _ring;

        private Animator _animator;
        private AnimatorSet _animatorSet;
        private bool _isAnimatorCancel;

        private IInterpolator _interpolator;
        //the ring's RectF
        private readonly RectF _tempBounds = new RectF();
        //绘制半圆的paint
        private readonly Paint _paint;
        private const int DefaultColor = 0x3B99DF;
        private bool _animationStarted;
        //the ring style
        private const int RingStyleSquare = 0;
        private const int RingStyleRound = 1;

        //the animator style
        private const int ProgressStyleMaterial = 0;
        private const int ProgressStyleLinear = 1;

        private float _rotation;

        public LoadingView(Context context, IAttributeSet attrs = null, int defStyleAttr = 0)
            : base(context, attrs, defStyleAttr)
        {
            _ring = new Ring();
            _bounds = new Rect();
            _paint = new Paint();
            _paint = new Paint(PaintFlags.AntiAlias);
            _paint.SetStyle(Paint.Style.Stroke);
            _paint.StrokeWidth = _ring.strokeWidth;

            _animatorListener = new CustomAnimatorListener(this);

            if (attrs == null) 
                return;
            
            var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.LoadingView, 0, 0);
            SetColor(a.GetInt(Resource.Styleable.LoadingView_loadding_color, DefaultColor));
            SetRingStyle(a.GetInt(Resource.Styleable.LoadingView_ring_style, RingStyleSquare));
            SetProgressStyle(a.GetInt(Resource.Styleable.LoadingView_progress_style, ProgressStyleMaterial));
            SetStrokeWidth(a.GetDimension(Resource.Styleable.LoadingView_ring_width, Dp2Px(StrokeWidth)));
            SetCenterRadius(a.GetDimension(Resource.Styleable.LoadingView_ring_radius, Dp2Px(CenterRadius)));
            a.Recycle();
        }


        /**
         * set the ring strokeWidth
         *
         * @param stroke
         */
        public void SetStrokeWidth(float stroke)
        {
            _ring.strokeWidth = stroke;
            _paint.StrokeWidth = stroke;
        }

        public void SetCenterRadius(float radius)
        {
            _ring.ringCenterRadius = radius;
        }

        public void SetRingStyle(int style)
        {
            switch (style)
            {
                case RingStyleSquare:
                    _paint.StrokeCap = Paint.Cap.Square;
                    break;
                case RingStyleRound:
                    _paint.StrokeCap = Paint.Cap.Round;
                    break;
            }
        }

        /**
         * set the animator's interpolator
         *
         * @param style
         */
        public void SetProgressStyle(int style)
        {
            switch (style)
            {
                case ProgressStyleMaterial:
                    _interpolator = new FastOutSlowInInterpolator();
                    break;
                case ProgressStyleLinear:
                    _interpolator = new LinearInterpolator();
                    break;
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            var widthSpecMode = MeasureSpec.GetMode(widthMeasureSpec);
            var widthSpecSize = MeasureSpec.GetSize(widthMeasureSpec);
            var heightSpecMode = MeasureSpec.GetMode(heightMeasureSpec);
            var heightSpecSize = MeasureSpec.GetSize(heightMeasureSpec);
            var width = (int)Dp2Px(CircleDiameter);
            var height = (int)Dp2Px(CircleDiameter);
                
            switch (widthSpecMode)
            {
                case MeasureSpecMode.AtMost when heightSpecMode == MeasureSpecMode.AtMost:
                    SetMeasuredDimension(width, height);
                    break;
                case MeasureSpecMode.AtMost:
                    SetMeasuredDimension(width, heightSpecSize);
                    break;
                default:
                {
                    if (heightSpecMode == MeasureSpecMode.AtMost)
                    {
                        SetMeasuredDimension(widthSpecSize, height);
                    }

                    break;
                }
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            var ring = _ring;
            ring.SetInsets(w, h);
            _bounds.Set(0, 0, w, h);
        }

        private void BuildAnimator()
        {
            var valueAnimator = ValueAnimator.OfFloat(0f, 1f);
            valueAnimator.SetDuration(AnimatorDuration);
            valueAnimator.RepeatCount = -1;
            valueAnimator.SetInterpolator(new LinearInterpolator());
            valueAnimator.Update += (sender, e) =>
            {
                _rotation = (float)valueAnimator.AnimatedValue;
                Invalidate();
            };
            _animator = valueAnimator;
            _animatorSet = BuildFlexibleAnimation();
            _animatorSet.AddListener(_animatorListener);
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (!_isAnimatorCancel)
            {
                var bounds = GetBounds();
                var saveCount = canvas.Save();
                canvas.Rotate(_rotation * 360, bounds.ExactCenterX(), bounds.ExactCenterY());
                DrawRing(canvas, bounds);
                canvas.RestoreToCount(saveCount);
            }
            else
            {
                canvas.Restore();
            }
        }

        /**
         * draw the ring
         *
         * @param canvas to draw the Ring
         * @param bounds the ring's rect
         */
        private void DrawRing(Canvas canvas, Rect bounds)
        {
            var arcBounds = _tempBounds;
            var ring = _ring;
            arcBounds.Set(bounds);
            arcBounds.Inset(ring.strokeInset, ring.strokeInset);
            canvas.DrawArc(arcBounds, ring.start, ring.sweep, false, _paint);
        }

        public void Start()
        {
            if (_animationStarted)
            {
                return;
            }

            if (_animator == null || _animatorSet == null)
            {
                _ring.Reset();
                BuildAnimator();
            }

            _animator.Start();
            _animatorSet.Start();
            _animationStarted = true;
            _isAnimatorCancel = false;
        }


        public void Stop()
        {
            _isAnimatorCancel = true;
            if (_animator != null)
            {
                _animator.End();
                _animator.Cancel();
            }
            if (_animatorSet != null)
            {

                _animatorSet.End();
                _animatorSet.Cancel();
            }
            _animator = null;
            _animatorSet = null;

            _animationStarted = false;
            _ring.Reset();
            _rotation = 0;
            Invalidate();
        }

        public Rect GetBounds()
        {
            return _bounds;
        }

        public void SetBounds(Rect bounds)
        {
            _bounds = bounds;
        }

        /**
         * build FlexibleAnimation to control the progress
         *
         * @return Animatorset for control the progress
         */
        private AnimatorSet BuildFlexibleAnimation()
        {
            var ring = _ring;
            var set = new AnimatorSet();
            var increment = ValueAnimator.OfFloat(0, MaxProgressArc - MinProgressArc);
            increment.SetDuration(AnimatorDuration / 2);
            increment.SetInterpolator(new LinearInterpolator());
            increment.Update += (sender, e) =>
            {
                var sweeping = ring.sweeping;
                var value = (float)e.Animation.AnimatedValue;
                ring.sweep = sweeping + value;
                Invalidate();
            };
            increment.AddListener(_animatorListener);
            var reduce = ValueAnimator.OfFloat(0, MaxProgressArc - MinProgressArc);
            reduce.SetDuration(AnimatorDuration / 2);
            reduce.SetInterpolator(_interpolator);
            reduce.Update += (sender, e) =>
            {
                var sweeping = ring.sweeping;
                var starting = ring.starting;
                var value = (float)e.Animation.AnimatedValue;
                ring.sweep = sweeping - value;
                ring.start = starting + value;
            };
            set.Play(reduce).After(increment);
            return set;
        }

        public void SetColor(int color)
        {
            _ring.color = color;
            _paint.Color = new Color(color);
        }

        public int GetColor()
        {
            return _ring.color;
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            Start();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            Stop();
        }

        protected override void OnVisibilityChanged(View changedView, ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);
            if (visibility == ViewStates.Visible)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        /**
         * turn dp to px
         *
         * @param dp value
         * @return result px value
         */
        private float Dp2Px(float dp)
        {
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, Resources.DisplayMetrics);
        }


        protected override IParcelable OnSaveInstanceState()
        {
            var parcelable = base.OnSaveInstanceState();
            var state = new SavedState(parcelable)
            {
                ring = _ring
            };
            return state;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            var savedState = (SavedState)state;
            base.OnRestoreInstanceState(state);
            _ring = savedState.ring;
        }

        protected class Ring : Object, IParcelable
        {
            public float strokeInset;
            public float strokeWidth;
            public float ringCenterRadius;
            public float start;
            public float end;
            public float sweep;
            public float sweeping = MinProgressArc;

            public float starting;
            public float ending;
            public int color;


            public void Restore()
            {
                starting = start;
                sweeping = sweep;
                ending = end;
            }

            public void Reset()
            {
                end = 0f;
                start = 0f;
                sweeping = MinProgressArc;
                sweep = 0f;
                starting = 0f;
            }

            public void SetInsets(int width, int height)
            {
                float minEdge = Math.Min(width, height);
                float insets;
                if (ringCenterRadius <= 0 || minEdge < 0)
                {
                    insets = (float)Math.Ceil(strokeWidth / 2.0f);
                }
                else
                {
                    insets = (minEdge / 2.0f - ringCenterRadius);
                }

                strokeInset = insets;
            }


            public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
            {
                dest.WriteFloat(strokeInset);
                dest.WriteFloat(strokeWidth);
                dest.WriteFloat(ringCenterRadius);
                dest.WriteFloat(start);
                dest.WriteFloat(end);
                dest.WriteFloat(sweep);
                dest.WriteFloat(sweeping);
                dest.WriteFloat(starting);
                dest.WriteFloat(ending);
                dest.WriteInt(color);
            }

            public int DescribeContents()
            {
                return 0;
            }

            public Ring()
            {
            }

            protected Ring(Parcel parcel)
            {
                strokeInset = parcel.ReadFloat();
                strokeWidth = parcel.ReadFloat();
                ringCenterRadius = parcel.ReadFloat();
                start = parcel.ReadFloat();
                end = parcel.ReadFloat();
                sweep = parcel.ReadFloat();
                sweeping = parcel.ReadFloat();
                starting = parcel.ReadFloat();
                ending = parcel.ReadFloat();
                color = parcel.ReadInt();
            }

            public static IParcelableCreator CREATOR = new CustomParcelableCreator();

            private class CustomParcelableCreator : Java.Lang.Object, IParcelableCreator
            {
                public Object CreateFromParcel(Parcel source)
                {
                    return new Ring(source);
                }

                public Object[] NewArray(int size)
                {
                    return new Ring[size];
                }
            }
        }

        protected class SavedState : BaseSavedState
        {
            public Ring ring;


            public SavedState(IParcelable superState)
                : base(superState)
            {
            }

            private SavedState(Parcel parcel) :
                    base(parcel)
            {
                ring = (Ring)parcel.ReadParcelable(Class.FromType(typeof(Ring)).ClassLoader);
            }

            public override void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
            {
                base.WriteToParcel(dest, flags);
                dest.WriteParcelable(ring, flags);
            }


            public static IParcelableCreator CREATOR = new CustomParcelableCreator();

            private class CustomParcelableCreator : Java.Lang.Object, IParcelableCreator
            {
                public Object CreateFromParcel(Parcel source)
                {
                    return new SavedState(source);
                }

                public Object[] NewArray(int size)
                {
                    return new SavedState[size];
                }
            }
        }

        private readonly IAnimatorListener _animatorListener;

        private class CustomAnimatorListener : Object, IAnimatorListener
        {
            private readonly LoadingView _loadingView;

            public CustomAnimatorListener(LoadingView loadingView)
            {
                _loadingView = loadingView;
            }

            public void OnAnimationCancel(Animator animation)
            {
            }

            public void OnAnimationEnd(Animator animation)
            {
                if (_loadingView._isAnimatorCancel) return;
                
                switch (animation)
                {
                    case ValueAnimator _:
                        _loadingView._ring.sweeping = _loadingView._ring.sweep;
                        break;
                    case AnimatorSet _:
                        _loadingView._ring.Restore();
                        _loadingView._animatorSet.Start();
                        break;
                }
            }

            public void OnAnimationRepeat(Animator animation)
            {
            }

            public void OnAnimationStart(Animator animation)
            {
            }
        }

    }
}