using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Support.V4.View.Animation;
using Android.Views;
using Android.Views.Animations;

namespace AutoLua.Droid.Views.RecyclerViews.Swipes
{
    /// <summary>
    /// 可绘制的材质进度
    /// </summary>
    [Register("AutoLua.Droid.Views.RecyclerViews.Swipes.MaterialProgressDrawable")]
    public class MaterialProgressDrawable : Drawable, IAnimatable
    {
        private static readonly IInterpolator LinearInterpolator = new LinearInterpolator();
        private static readonly IInterpolator MaterialInterpolator = new FastOutSlowInInterpolator();

        private const float FullRotation = 1080.0f;

        // Maps to ProgressBar.Large style
        public const int Large = 0;

        // Maps to ProgressBar default style
        public const int Default = 1;

        // Maps to ProgressBar default style
        private const int CircleDiameter = 40;
        private const float CenterRadius = 8.75f; //should add up to 10 when + stroke_width
        private const float StrokeWidth = 2.5f;

        // Maps to ProgressBar.Large style
        private const int CircleDiameterLarge = 56;
        private const float CenterRadiusLarge = 12.5f;
        private const float StrokeWidthLarge = 3f;

        private readonly int[] _colors = {
            Color.Black
        };

        /**
         * The value in the linear interpolator for animating the drawable at which
         * the color transition should start
         */
        private const float ColorStartDelayOffset = 0.75f;

        private const float EndTrimStartDelayOffset = 0.5f;
        private const float StartTrimDurationOffset = 0.5f;

        /** The duration of a single progress spin in milliseconds. */
        private const int AnimationDuration = 1332;

        /** The number of points in the progress "star". */
        private const float NumPoints = 5f;

        /** The list of animators operating on this drawable. */
        private readonly List<Animation> _animators = new List<Animation>();

        /** The indicator ring, used to manage animation state. */
        private readonly Ring _ring;

        /** Canvas rotation in degrees. */
        private float _rotation;

        /** Layout info for the arrowhead in dp */
        private const int ArrowWidth = 10;

        private const int ArrowHeight = 5;
        private const float ArrowOffsetAngle = 5;

        /** Layout info for the arrowhead for the large spinner in dp */
        private const int ArrowWidthLarge = 12;

        private const int ArrowHeightLarge = 6;
        private const float MaxProgressArc = .8f;

        private readonly Resources _resources;
        private readonly View _parent;
        private Animation _animation;
        private float _rotationCount;
        private double _width;
        private double _height;
        private bool _finishing;

        public MaterialProgressDrawable(Context context, View parent)
        {
            _parent = parent;
            _resources = context.Resources;
            ICallback callback = new CustomCallback(this);
            _ring = new Ring(callback);
            _ring.SetColors(_colors);

            UpdateSizes(Default);
            SetupAnimators();
        }

        public MaterialProgressDrawable(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        private void SetSizeParameters(double progressCircleWidth, double progressCircleHeight,
            double centerRadius, double strokeWidth, float arrowWidth, float arrowHeight)
        {
            var ring = _ring;
            var metrics = _resources.DisplayMetrics;
            var screenDensity = metrics.Density;

            _width = progressCircleWidth * screenDensity;
            _height = progressCircleHeight * screenDensity;
            ring.SetStrokeWidth((float) strokeWidth * screenDensity);
            ring.SetCenterRadius(centerRadius * screenDensity);
            ring.SetColorIndex(0);
            ring.SetArrowDimensions(arrowWidth * screenDensity, arrowHeight * screenDensity);
            ring.SetInsets((int) _width, (int) _height);
        }

        /**
         * Set the overall size for the progress spinner. This updates the radius
         * and stroke width of the ring.
         *
         * @param size One of {MaterialProgressDrawable.LARGE} or
         *            {MaterialProgressDrawable.DEFAULT}
         */
        public void UpdateSizes(int size)
        {
            if (size == Large)
            {
                SetSizeParameters(CircleDiameterLarge, CircleDiameterLarge, CenterRadiusLarge,
                    StrokeWidthLarge, ArrowWidthLarge, ArrowHeightLarge);
            }
            else
            {
                SetSizeParameters(CircleDiameter, CircleDiameter, CenterRadius, StrokeWidth,
                    ArrowWidth, ArrowHeight);
            }
        }

        /**
         * @param show Set to true to display the arrowhead on the progress spinner.
         */
        public void ShowArrow(bool show)
        {
            _ring.SetShowArrow(show);
        }

        /**
         * @param scale Set the scale of the arrowhead for the spinner.
         */
        public void SetArrowScale(float scale)
        {
            _ring.SetArrowScale(scale);
        }

        /**
         * Set the start and end trim for the progress spinner arc.
         *
         * @param startAngle start angle
         * @param endAngle end angle
         */
        public void SetStartEndTrim(float startAngle, float endAngle)
        {
            _ring.SetStartTrim(startAngle);
            _ring.SetEndTrim(endAngle);
        }

        /**
         * Set the amount of rotation to apply to the progress spinner.
         *
         * @param rotation Rotation is from [0..1]
         */
        public void SetProgressRotation(float rotation)
        {
            _ring.SetRotation(rotation);
        }

        /**
         * Update the background color of the circle image view.
         */
        public void SetBackgroundColor(int color)
        {
            _ring.SetBackgroundColor(color);
        }

        /**
         * Set the colors used in the progress animation from color resources.
         * The first color will also be the color of the bar that grows in response
         * to a user swipe gesture.
         *
         * @param colors
         */
        public void SetColorSchemeColors(params int[] colors)
        {
            _ring.SetColors(colors);
            _ring.SetColorIndex(0);
        }

        public override int IntrinsicHeight => (int) _height;

        public override int IntrinsicWidth => (int) _width;

        public override int Alpha
        {
            get => _ring.GetAlpha();
            set => _ring.SetAlpha(value);
        }

        public override void SetColorFilter(ColorFilter colorFilter)
        {
            _ring.SetColorFilter(colorFilter);
        }

        [Java.Lang.SuppressWarnings(Value = new[] {"unused"})]
        private void SetRotation(float rotation)
        {
            _rotation = rotation;
            InvalidateSelf();
        }

        [Java.Lang.SuppressWarnings(Value = new[] {"unused"})]
        private float GetRotation()
        {
            return _rotation;
        }

        private static float GetMinProgressArc(Ring ring)
        {
            return (float) Java.Lang.Math.ToRadians(
                ring.GetStrokeWidth() / (2 * Math.PI * ring.GetCenterRadius()));
        }

        // Adapted from ArgbEvaluator.java
        private static int EvaluateColorChange(float fraction, int startValue, int endValue)
        {
            var startInt = startValue;
            var startA = (startInt >> 24) & 0xff;
            var startR = (startInt >> 16) & 0xff;
            var startG = (startInt >> 8) & 0xff;
            var startB = startInt & 0xff;

            var endInt = endValue;
            var endA = (endInt >> 24) & 0xff;
            var endR = (endInt >> 16) & 0xff;
            var endG = (endInt >> 8) & 0xff;
            var endB = endInt & 0xff;

            return (startA + (int) (fraction * (endA - startA))) << 24 |
                   (startR + (int) (fraction * (endR - startR))) << 16 |
                   (startG + (int) (fraction * (endG - startG))) << 8 |
                   (startB + (int) (fraction * (endB - startB)));
        }

        /**
         * Update the ring color if this is within the last 25% of the animation.
         * The new ring color will be a translation from the starting ring color to
         * the next color.
         */
        private static void UpdateRingColor(float interpolatedTime, Ring ring)
        {
            if (interpolatedTime > ColorStartDelayOffset)
            {
                // scale the interpolatedTime so that the full
                // transformation from 0 - 1 takes place in the
                // remaining time
                ring.SetColor(EvaluateColorChange((interpolatedTime - ColorStartDelayOffset)
                                                  / (1.0f - ColorStartDelayOffset), ring.GetStartingColor(),
                    ring.GetNextColor()));
            }
        }

        private static void ApplyFinishTranslation(float interpolatedTime, Ring ring)
        {
            // shrink back down and complete a full rotation before
            // starting other circles
            // Rotation goes between [0..1].
            UpdateRingColor(interpolatedTime, ring);
            var targetRotation = (float) (Math.Floor(ring.GetStartingRotation() / MaxProgressArc)
                                          + 1f);
            var minProgressArc = GetMinProgressArc(ring);
            var startTrim = ring.GetStartingStartTrim()
                            + (ring.GetStartingEndTrim() - minProgressArc - ring.GetStartingStartTrim())
                            * interpolatedTime;
            ring.SetStartTrim(startTrim);
            ring.SetEndTrim(ring.GetStartingEndTrim());
            var rotation = ring.GetStartingRotation()
                           + ((targetRotation - ring.GetStartingRotation()) * interpolatedTime);
            ring.SetRotation(rotation);
        }

        private void SetupAnimators()
        {
            Animation animation = new CustomAnimation(this);

            animation.RepeatCount = Animation.Infinite;
            animation.RepeatMode = RepeatMode.Restart;
            animation.Interpolator = LinearInterpolator;
            animation.SetAnimationListener(new CustomAnimationListener(this));

            _animation = animation;
        }

        public void Start()
        {
            _animation.Reset();
            _ring.StoreOriginals();
            // Already showing some part of the ring
            if (_ring.GetEndTrim() != _ring.GetStartTrim())
            {
                _finishing = true;
                _animation.Duration = (AnimationDuration / 2);
                _parent.StartAnimation(_animation);
            }
            else
            {
                _ring.SetColorIndex(0);
                _ring.ResetOriginals();
                _animation.Duration = AnimationDuration;
                _parent.StartAnimation(_animation);
            }
        }

        public void Stop()
        {
            _parent.ClearAnimation();
            SetRotation(0);
            _ring.SetShowArrow(false);
            _ring.SetColorIndex(0);
            _ring.ResetOriginals();
        }

        public override void Draw(Canvas c)
        {
            var bounds = Bounds;
            var saveCount = c.Save();
            c.Rotate(_rotation, bounds.ExactCenterX(), bounds.ExactCenterY());
            _ring.Draw(c, bounds);
            c.RestoreToCount(saveCount);
        }

        public override void SetAlpha(int alpha)
        {
            _ring.SetAlpha(alpha);
        }

        private class Ring
        {
            private readonly RectF _tempBounds = new RectF();
            private readonly Paint _paint = new Paint();
            private readonly Paint _arrowPaint = new Paint();

            private readonly ICallback _callback;

            private float _startTrim;
            private float _endTrim;
            private float _rotation;
            private float _strokeWidth = 5.0f;
            private float _strokeInset = 2.5f;

            private int[] _colors;

            // mColorIndex represents the offset into the available mColors that the
            // progress circle should currently display. As the progress circle is
            // animating, the mColorIndex moves by one to the next available color.
            private int _colorIndex;
            private float _startingStartTrim;
            private float _startingEndTrim;
            private float _startingRotation;
            private bool _showArrow;
            private Path _arrow;
            private float _arrowScale;
            private double _ringCenterRadius;
            private int _arrowWidth;
            private int _arrowHeight;
            private int _alpha;
            private readonly Paint _circlePaint = new Paint {Flags = PaintFlags.AntiAlias};
            private int _backgroundColor;
            private int _currentColor;

            public Ring(ICallback callback)
            {
                _callback = callback;

                _paint.StrokeCap = Paint.Cap.Square;
                _paint.AntiAlias = (true);
                _paint.SetStyle(Paint.Style.Stroke);

                _arrowPaint.SetStyle(Paint.Style.Fill);
                _arrowPaint.AntiAlias = (true);
            }

            public void SetBackgroundColor(int color)
            {
                _backgroundColor = color;
            }

            /**
             * Set the dimensions of the arrowhead.
             *
             * @param width Width of the hypotenuse of the arrow head
             * @param height Height of the arrow point
             */
            public void SetArrowDimensions(float width, float height)
            {
                _arrowWidth = (int) width;
                _arrowHeight = (int) height;
            }

            /**
             * Draw the progress spinner
             */
            public void Draw(Canvas c, Rect bounds)
            {
                var arcBounds = _tempBounds;
                arcBounds.Set(bounds);
                arcBounds.Inset(_strokeInset, _strokeInset);

                var startAngle = (_startTrim + _rotation) * 360;
                var endAngle = (_endTrim + _rotation) * 360;
                var sweepAngle = endAngle - startAngle;

                _paint.Color = new Color(_currentColor);
                c.DrawArc(arcBounds, startAngle, sweepAngle, false, _paint);

                DrawTriangle(c, startAngle, sweepAngle, bounds);

                if (_alpha >= 255) 
                    return;
                
                _circlePaint.Color = new Color(_backgroundColor);
                _circlePaint.Alpha = (255 - _alpha);
                c.DrawCircle(bounds.ExactCenterX(), bounds.ExactCenterY(), bounds.Width() / 2,
                    _circlePaint);
            }

            private void DrawTriangle(Canvas c, float startAngle, float sweepAngle, Rect bounds)
            {
                if (!_showArrow) 
                    return;
                
                if (_arrow == null)
                {
                    _arrow = new Path();
                    _arrow.SetFillType(Path.FillType.EvenOdd);
                }
                else
                {
                    _arrow.Reset();
                }

                // Adjust the position of the triangle so that it is inset as
                // much as the arc, but also centered on the arc.
                var inset = (int) _strokeInset / 2 * _arrowScale;
                var x = (float) (_ringCenterRadius * Math.Cos(0) + bounds.ExactCenterX());
                var y = (float) (_ringCenterRadius * Math.Sin(0) + bounds.ExactCenterY());

                // Update the path each time. This works around an issue in SKIA
                // where concatenating a rotation matrix to a scale matrix
                // ignored a starting negative rotation. This appears to have
                // been fixed as of API 21.
                _arrow.MoveTo(0, 0);
                _arrow.LineTo(_arrowWidth * _arrowScale, 0);
                _arrow.LineTo((_arrowWidth * _arrowScale / 2), (_arrowHeight
                                                                * _arrowScale));
                _arrow.Offset(x - inset, y);
                _arrow.Close();
                // draw a triangle
                _arrowPaint.Color = new Color(_currentColor);
                c.Rotate(startAngle + sweepAngle - ArrowOffsetAngle, bounds.ExactCenterX(),
                    bounds.ExactCenterY());
                c.DrawPath(_arrow, _arrowPaint);
            }

            /**
             * Set the colors the progress spinner alternates between.
             *
             * @param colors Array of integers describing the colors. Must be non-<code>null</code>.
             */
            public void SetColors(int[] colors)
            {
                _colors = colors;
                // if colors are reset, make sure to reset the color index as well
                SetColorIndex(0);
            }

            /**
             * Set the absolute color of the progress spinner. This is should only
             * be used when animating between current and next color when the
             * spinner is rotating.
             *
             * @param color int describing the color.
             */
            public void SetColor(int color)
            {
                _currentColor = color;
            }

            /**
             * @param index Index into the color array of the color to display in
             *            the progress spinner.
             */
            public void SetColorIndex(int index)
            {
                _colorIndex = index;
                _currentColor = _colors[_colorIndex];
            }

            /**
             * @return int describing the next color the progress spinner should use when drawing.
             */
            public int GetNextColor()
            {
                return _colors[GetNextColorIndex()];
            }

            private int GetNextColorIndex()
            {
                return (_colorIndex + 1) % (_colors.Length);
            }

            /**
             * Proceed to the next available ring color. This will automatically
             * wrap back to the beginning of colors.
             */
            public void GoToNextColor()
            {
                SetColorIndex(GetNextColorIndex());
            }

            public void SetColorFilter(ColorFilter filter)
            {
                _paint.SetColorFilter(filter);
                InvalidateSelf();
            }

            /**
             * @param alpha Set the alpha of the progress spinner and associated arrowhead.
             */
            public void SetAlpha(int alpha)
            {
                _alpha = alpha;
            }

            /**
             * @return Current alpha of the progress spinner and arrowhead.
             */
            public int GetAlpha()
            {
                return _alpha;
            }

            /**
             * @param strokeWidth Set the stroke width of the progress spinner in pixels.
             */
            public void SetStrokeWidth(float strokeWidth)
            {
                _strokeWidth = strokeWidth;
                _paint.StrokeWidth = strokeWidth;
                InvalidateSelf();
            }

            [Java.Lang.SuppressWarnings(Value = new[] {"unused"})]
            public float GetStrokeWidth()
            {
                return _strokeWidth;
            }

            [Java.Lang.SuppressWarnings(Value = new[] {"unused"})]
            public void SetStartTrim(float startTrim)
            {
                _startTrim = startTrim;
                InvalidateSelf();
            }

            [Java.Lang.SuppressWarnings(Value = new[] {"unused"})]
            public float GetStartTrim()
            {
                return _startTrim;
            }

            public float GetStartingStartTrim()
            {
                return _startingStartTrim;
            }

            public float GetStartingEndTrim()
            {
                return _startingEndTrim;
            }

            public int GetStartingColor()
            {
                return _colors[_colorIndex];
            }

            [Java.Lang.SuppressWarnings(Value = new[] {"unused"})]
            public void SetEndTrim(float endTrim)
            {
                _endTrim = endTrim;
                InvalidateSelf();
            }

            [Java.Lang.SuppressWarnings(Value = new[] {"unused"})]
            public float GetEndTrim()
            {
                return _endTrim;
            }

            [Java.Lang.SuppressWarnings(Value = new[] {"unused"})]
            public void SetRotation(float rotation)
            {
                _rotation = rotation;
                InvalidateSelf();
            }

            [Java.Lang.SuppressWarnings(Value = new[] {"unused"})]
            public float GetRotation()
            {
                return _rotation;
            }

            public void SetInsets(int width, int height)
            {
                var minEdge = (float) Math.Min(width, height);
                float insets;
                if (_ringCenterRadius <= 0 || minEdge < 0)
                {
                    insets = (float) Math.Ceiling(_strokeWidth / 2.0f);
                }
                else
                {
                    insets = (float) (minEdge / 2.0f - _ringCenterRadius);
                }

                _strokeInset = insets;
            }

            public void SetCenterRadius(double centerRadius)
            {
                _ringCenterRadius = centerRadius;
            }

            public double GetCenterRadius()
            {
                return _ringCenterRadius;
            }

            /**
             * @param show Set to true to show the arrow head on the progress spinner.
             */
            public void SetShowArrow(bool show)
            {
                if (_showArrow == show) 
                    return;
                
                _showArrow = show;
                InvalidateSelf();
            }

            /**
             * @param scale Set the scale of the arrowhead for the spinner.
             */
            public void SetArrowScale(float scale)
            {
                if (scale == _arrowScale) 
                    return;
                
                _arrowScale = scale;
                InvalidateSelf();
            }

            /**
             * @return The amount the progress spinner is currently rotated, between [0..1].
             */
            public float GetStartingRotation()
            {
                return _startingRotation;
            }

            /**
             * If the start / end trim are offset to begin with, store them so that
             * animation starts from that offset.
             */
            public void StoreOriginals()
            {
                _startingStartTrim = _startTrim;
                _startingEndTrim = _endTrim;
                _startingRotation = _rotation;
            }

            /**
             * Reset the progress spinner to default rotation, start and end angles.
             */
            public void ResetOriginals()
            {
                _startingStartTrim = 0;
                _startingEndTrim = 0;
                _startingRotation = 0;
                SetStartTrim(0);
                SetEndTrim(0);
                SetRotation(0);
            }

            private void InvalidateSelf()
            {
                _callback.InvalidateDrawable(null);
            }
        }

        private class CustomCallback : Java.Lang.Object, ICallback
        {
            private readonly MaterialProgressDrawable _materialProgressDrawable;

            public CustomCallback(MaterialProgressDrawable materialProgressDrawable)
            {
                _materialProgressDrawable = materialProgressDrawable;
            }

            public void InvalidateDrawable(Drawable who)
            {
                _materialProgressDrawable.InvalidateSelf();
            }

            public void ScheduleDrawable(Drawable who, Java.Lang.IRunnable what, long when)
            {
                _materialProgressDrawable.ScheduleSelf(what, when);
            }

            public void UnscheduleDrawable(Drawable who, Java.Lang.IRunnable what)
            {
                _materialProgressDrawable.UnscheduleSelf(what);
            }
        }

        private class CustomAnimation : Animation
        {
            private readonly MaterialProgressDrawable _materialProgressDrawable;

            public CustomAnimation(MaterialProgressDrawable materialProgressDrawable)
            {
                _materialProgressDrawable = materialProgressDrawable;
            }

            protected override void ApplyTransformation(float interpolatedTime, Transformation t)
            {
                var ring = _materialProgressDrawable._ring;
                if (_materialProgressDrawable._finishing)
                {
                    ApplyFinishTranslation(interpolatedTime, ring);
                }
                else
                {
                    // The minProgressArc is calculated from 0 to create an
                    // angle that matches the stroke width.
                    var minProgressArc = GetMinProgressArc(ring);
                    var startingEndTrim = ring.GetStartingEndTrim();
                    var startingTrim = ring.GetStartingStartTrim();
                    var startingRotation = ring.GetStartingRotation();

                    UpdateRingColor(interpolatedTime, ring);

                    // Moving the start trim only occurs in the first 50% of a
                    // single ring animation
                    if (interpolatedTime <= StartTrimDurationOffset)
                    {
                        // scale the interpolatedTime so that the full
                        // transformation from 0 - 1 takes place in the
                        // remaining time
                        var scaledTime = (interpolatedTime)
                                         / (1.0f - StartTrimDurationOffset);
                        var startTrim = startingTrim
                                        + ((MaxProgressArc - minProgressArc) * MaterialInterpolator
                                            .GetInterpolation(scaledTime));
                        ring.SetStartTrim(startTrim);
                    }

                    // Moving the end trim starts after 50% of a single ring
                    // animation completes
                    if (interpolatedTime > EndTrimStartDelayOffset)
                    {
                        // scale the interpolatedTime so that the full
                        // transformation from 0 - 1 takes place in the
                        // remaining time
                        var minArc = MaxProgressArc - minProgressArc;
                        var scaledTime = (interpolatedTime - StartTrimDurationOffset)
                                         / (1.0f - StartTrimDurationOffset);
                        var endTrim = startingEndTrim
                                      + (minArc * MaterialInterpolator.GetInterpolation(scaledTime));
                        ring.SetEndTrim(endTrim);
                    }

                    var rotation = startingRotation + (0.25f * interpolatedTime);
                    ring.SetRotation(rotation);

                    var groupRotation = ((FullRotation / NumPoints) * interpolatedTime)
                                        + (FullRotation * (_materialProgressDrawable._rotationCount / NumPoints));
                    _materialProgressDrawable.SetRotation(groupRotation);
                }
            }
        }

        private class CustomAnimationListener : Java.Lang.Object, Animation.IAnimationListener
        {
            private readonly MaterialProgressDrawable _materialProgressDrawable;

            public CustomAnimationListener(MaterialProgressDrawable materialProgressDrawable)
            {
                _materialProgressDrawable = materialProgressDrawable;
            }

            public void OnAnimationStart(Animation animation)
            {
                _materialProgressDrawable._rotationCount = 0;
            }

            public void OnAnimationEnd(Animation animation)
            {
                // do nothing
            }

            public void OnAnimationRepeat(Animation animation)
            {
                var ring = _materialProgressDrawable._ring;
                ring.StoreOriginals();
                ring.GoToNextColor();
                ring.SetStartTrim(ring.GetEndTrim());
                if (_materialProgressDrawable._finishing)
                {
                    // finished closing the last ring from the swipe gesture; go
                    // into progress mode
                    _materialProgressDrawable._finishing = false;
                    animation.Duration = (AnimationDuration);
                    ring.SetShowArrow(false);
                }
                else
                {
                    _materialProgressDrawable._rotationCount =
                        (_materialProgressDrawable._rotationCount + 1) % (NumPoints);
                }
            }
        }

        public override int Opacity => (int) Format.Transparent;

        public bool IsRunning
        {
            get
            {
                var animators = _animators;
                var n = animators.Count();
                for (var i = 0; i < n; i++)
                {
                    var animator = animators[i];
                    if (animator.HasStarted && !animator.HasEnded)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}