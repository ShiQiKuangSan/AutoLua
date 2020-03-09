using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Views.Animations;
using static Android.Views.Animations.Animation;
using Android.Util;
using Android.Support.V4.View;
using Java.Lang;
using Android.Content.Res;
using Android.Support.V7.Widget;
using AndroidResource = Android.Resource;
using Android.Support.V4.Content;

namespace AutoLua.Droid.Views.RecyclerViews.Swipes
{
#pragma warning disable 618
    [Register("AutoLua.Droid.Views.RecyclerViews.Swipes.SwipeRefreshLayout")]
    public sealed class SwipeRefreshLayout : FrameLayout
    {
        private const string LogTag = "SwipeRefreshLayout"; // SwipeRefreshLayout.Class.getSimpleName();

        private const int MaxAlpha = 255;
        private const int StartingProgressAlpha = (int) (.3f * MaxAlpha);

        private const int CircleDiameter = 40;
        private const int CircleDiameterLarge = 56;

        private const float DecelerateInterpolationFactor = 2f;
        private const int InvalidPointer = -1;
        private const float DragRate = .5f;

        // Max amount of circle that can be filled by progress during swipe gesture,
        // where 1.0 is a full circle
        private const float MaxProgressAngle = .8f;

        private const int ScaleDownDuration = 150;

        private const int AlphaAnimationDuration = 300;

        private const int AnimateToTriggerDuration = 200;

        private const int AnimateToStartDuration = 200;

        // Default background for the progress spinner
        private const int CircleBgLight = 0xFAFAFA;

        // Default offset in dips from the top of the view to where the progress spinner should stop
        private const int DefaultCircleTarget = 64;

        private View _target; // the target of the gesture
        private IOnRefreshListener _listener;
        private bool _refreshing;
        private readonly int _touchSlop;
        private float _totalDragDistance;
        private readonly int _mediumAnimationDuration;

        private int _currentTargetOffsetTop;

        // Whether or not the starting offset has been determined.
        private bool _originalOffsetCalculated;

        private float _initialMotionY;
        private float _initialDownY;
        private bool _isBeingDragged;

        private int _activePointerId = InvalidPointer;

        // Whether this item is scaled up rather than clipped
        private bool _scale;

        // Target is returning to its start offset because it was cancelled or a
        // refresh was triggered.
        private bool _returningToStart;
        private readonly DecelerateInterpolator _decelerateInterpolator;

        private static readonly int[] LayoutAttrs = {
            AndroidResource.Attribute.Enabled
        };

        private CircleImageView _circleView;
        private int _circleViewIndex = -1;

        protected int From;

        private float _startingScale;

        protected int OriginalOffsetTop;

        private MaterialProgressDrawable _progress;

        private Animation _scaleAnimation;

        private Animation _scaleDownAnimation;

        private Animation _alphaStartAnimation;

        private Animation _alphaMaxAnimation;

        private Animation _scaleDownToStartAnimation;

        private float _spinnerFinalOffset;

        private bool _notify;

        private int _circleWidth;

        private int _circleHeight;

        // Whether the client has set a custom starting position;
        private bool _usingCustomStart;
        private readonly IAnimationListener _refreshListener;


        /**
         * Constructor that is called when inflating SwipeRefreshLayout from XML.
         *
         * @param context
         * @param attrs
         */
        public SwipeRefreshLayout(Context context, IAttributeSet attrs = null)
            : base(context, attrs)
        {
            _refreshListener = new CustomRefreshListener(this);
            _animateToCorrectPosition = new CustommAnimateToCorrectPosition(this);
            _animateToStartPosition = new CustommAnimateToStartPosition(this);

            _touchSlop = ViewConfiguration.Get(context).ScaledTouchSlop;

            _mediumAnimationDuration = Resources.GetInteger(
                AndroidResource.Integer.ConfigMediumAnimTime);

            SetWillNotDraw(false);
            _decelerateInterpolator = new DecelerateInterpolator(DecelerateInterpolationFactor);

            var a = context.ObtainStyledAttributes(attrs, LayoutAttrs);
            Enabled = (a.GetBoolean(0, true));
            a.Recycle();

            var metrics = Resources.DisplayMetrics;
            _circleWidth = (int) (CircleDiameter * metrics.Density);
            _circleHeight = (int) (CircleDiameter * metrics.Density);

            CreateProgressView();
#pragma warning disable 618
            ViewCompat.SetChildrenDrawingOrderEnabled(this, true);
#pragma warning restore 618
            // the absolute offset has to take into account that the circle starts at an offset
            _spinnerFinalOffset = DefaultCircleTarget * metrics.Density;
            _totalDragDistance = _spinnerFinalOffset;

            RequestDisallowInterceptTouchEvent(true);
        }

        private void SetColorViewAlpha(int targetAlpha)
        {
            _circleView.Background.SetAlpha(targetAlpha);
            _progress.SetAlpha(targetAlpha);
        }

        /**
         * The refresh indicator starting and resting position is always positioned
         * near the top of the refreshing content. This position is a consistent
         * location, but can be adjusted in either direction based on whether or not
         * there is a toolbar or actionbar present.
         *
         * @param scale Set to true if there is no view at a higher z-order than
         *            where the progress spinner is set to appear.
         * @param start The offset in pixels from the top of this view at which the
         *            progress spinner should appear.
         * @param end The offset in pixels from the top of this view at which the
         *            progress spinner should come to rest after a successful swipe
         *            gesture.
         */
        public void SetProgressViewOffset(bool scale, int start, int end)
        {
            _scale = scale;
            _circleView.Visibility = ViewStates.Gone;
            OriginalOffsetTop = _currentTargetOffsetTop = start;
            _spinnerFinalOffset = end;
            _usingCustomStart = true;
            _circleView.Invalidate();
        }

        /**
         * The refresh indicator resting position is always positioned near the top
         * of the refreshing content. This position is a consistent location, but
         * can be adjusted in either direction based on whether or not there is a
         * toolbar or actionbar present.
         *
         * @param scale Set to true if there is no view at a higher z-order than
         *            where the progress spinner is set to appear.
         * @param end The offset in pixels from the top of this view at which the
         *            progress spinner should come to rest after a successful swipe
         *            gesture.
         */
        public void SetProgressViewEndTarget(bool scale, int end)
        {
            _spinnerFinalOffset = end;
            _scale = scale;
            _circleView.Invalidate();
        }

        /**
         * One of DEFAULT, or LARGE.
         */
        public void SetSize(int size)
        {
            if (size != MaterialProgressDrawable.Large && size != MaterialProgressDrawable.Default)
            {
                return;
            }

            var metrics = Resources.DisplayMetrics;
            if (size == MaterialProgressDrawable.Large)
            {
                _circleHeight = _circleWidth = (int) (CircleDiameterLarge * metrics.Density);
            }
            else
            {
                _circleHeight = _circleWidth = (int) (CircleDiameter * metrics.Density);
            }

            // force the bounds of the progress circle inside the circle view to
            // update by setting it to null before updating its size and then
            // re-setting it
            _circleView.SetImageDrawable(null);
            _progress.UpdateSizes(size);
            _circleView.SetImageDrawable(_progress);
        }

        protected int getChildDrawingOrder(int childCount, int i)
        {
            if (_circleViewIndex < 0)
            {
                return i;
            }

            if (i == childCount - 1)
            {
                // Draw the selected child last
                return _circleViewIndex;
            }

            if (i >= _circleViewIndex)
            {
                // Move the children after the selected child earlier one
                return i + 1;
            }

            // Keep the children before the selected child the same
            return i;
        }

        private void CreateProgressView()
        {
            _circleView = new CircleImageView(Context, CircleBgLight, CircleDiameter / 2);
            _progress = new MaterialProgressDrawable(Context, this);
            _progress.SetBackgroundColor(CircleBgLight);
            _circleView.SetImageDrawable(_progress);
            _circleView.Visibility = ViewStates.Gone;
            AddView(_circleView);
        }

        /**
         * Set the listener to be notified when a refresh is triggered via the swipe
         * gesture.
         */
        public void SetOnRefreshListener(IOnRefreshListener listener)
        {
            _listener = listener;
        }

        /**
         * Pre API 11, alpha is used to make the progress circle appear instead of scale.
         */
        private static bool IsAlphaUsedForScale()
        {
            return Build.VERSION.SdkInt < BuildVersionCodes.Honeycomb; // 11;
        }

        /**
         * Notify the widget that refresh state has changed. Do not call this when
         * refresh is triggered by a swipe gesture.
         *
         * @param refreshing Whether or not the view should show refresh progress.
         */
        public void SetRefreshing(bool refreshing)
        {
            if (refreshing && _refreshing != true)
            {
                // scale and show
                _refreshing = true;
                var endTarget = 0;
                if (!_usingCustomStart)
                {
                    endTarget = (int) (_spinnerFinalOffset + OriginalOffsetTop);
                }
                else
                {
                    endTarget = (int) _spinnerFinalOffset;
                }

                SetTargetOffsetTopAndBottom(endTarget - _currentTargetOffsetTop,
                    true /* requires update */);
                _notify = false;
                StartScaleUpAnimation(_refreshListener);
            }
            else
            {
                SetRefreshing(refreshing, false /* notify */);
            }
        }

        private void StartScaleUpAnimation(IAnimationListener listener)
        {
            _circleView.Visibility = ViewStates.Visible;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                // 11
                // Pre API 11, alpha is used in place of scale up to show the
                // progress circle appearing.
                // Don't adjust the alpha during appearance otherwise.
                _progress.SetAlpha(MaxAlpha);
            }

            _scaleAnimation = new CustomScaleAnimation(this)
            {
                Duration = _mediumAnimationDuration
            };
            
            if (listener != null)
            {
                _circleView.SetAnimationListener(listener);
            }

            _circleView.ClearAnimation();
            _circleView.StartAnimation(_scaleAnimation);
        }

        /**
         * Pre API 11, this does an alpha animation.
         * @param progress
         */
        private void SetAnimationProgress(float progress)
        {
            if (IsAlphaUsedForScale())
            {
                SetColorViewAlpha((int) (progress * MaxAlpha));
            }
            else
            {
#pragma warning disable 618
                ViewCompat.SetScaleX(_circleView, progress);
                ViewCompat.SetScaleY(_circleView, progress);
#pragma warning restore 618
            }
        }

        private void SetRefreshing(bool refreshing, bool notify)
        {
            if (_refreshing == refreshing) 
                return;
            
            _notify = notify;
            EnsureTarget();
            _refreshing = refreshing;
            if (_refreshing)
            {
                AnimateOffsetToCorrectPosition(_currentTargetOffsetTop, _refreshListener);
            }
            else
            {
                StartScaleDownAnimation(_refreshListener);
            }
        }

        private void StartScaleDownAnimation(IAnimationListener listener)
        {
            _scaleDownAnimation = new CustommScaleDownAnimation(this)
            {
                Duration = ScaleDownDuration
            };
            
            _circleView.SetAnimationListener(listener);
            _circleView.ClearAnimation();
            _circleView.StartAnimation(_scaleDownAnimation);
        }

        private void StartProgressAlphaStartAnimation()
        {
            _alphaStartAnimation = StartAlphaAnimation(_progress.Alpha, StartingProgressAlpha);
        }

        private void StartProgressAlphaMaxAnimation()
        {
            _alphaMaxAnimation = StartAlphaAnimation(_progress.Alpha, MaxAlpha);
        }

        private Animation StartAlphaAnimation(int startingAlpha, int endingAlpha)
        {
            // Pre API 11, alpha is used in place of scale. Don't also use it to
            // show the trigger point.
            if (_scale && IsAlphaUsedForScale())
            {
                return null;
            }

            Animation alpha = new CustomAlphaAnimation(this, startingAlpha, endingAlpha);

            alpha.Duration = (AlphaAnimationDuration);
            // Clear out the previous animation listeners.
            _circleView.SetAnimationListener(null);
            _circleView.ClearAnimation();
            _circleView.StartAnimation(alpha);
            return alpha;
        }

        /**
         * @deprecated Use {@link #setProgressBackgroundColorSchemeResource(int)}
         */
        [Deprecated]
        public void SetProgressBackgroundColor(int colorRes)
        {
            SetProgressBackgroundColorSchemeResource(colorRes);
        }

        /**
         * Set the background color of the progress spinner disc.
         *
         * @param colorRes Resource id of the color.
         */
        public void SetProgressBackgroundColorSchemeResource(int colorRes)
        {
            SetProgressBackgroundColorSchemeColor(ContextCompat.GetColor(Context, colorRes));
        }

        /**
         * Set the background color of the progress spinner disc.
         *
         * @param color
         */
        public void SetProgressBackgroundColorSchemeColor(int color)
        {
            _circleView.SetBackgroundColor(color);
            _progress.SetBackgroundColor(color);
        }

        /**
         * @deprecated Use {@link #setColorSchemeResources(int...)}
         */
        [Deprecated]
        public void SetColorScheme(int[] colors)
        {
            SetColorSchemeResources(colors);
        }

        /**
         * Set the color resources used in the progress animation from color resources.
         * The first color will also be the color of the bar that grows in response
         * to a user swipe gesture.
         *
         * @param colorResIds
         */
        public void SetColorSchemeResources(int[] colorResIds)
        {
            var colorRes = new int[colorResIds.Length];
            for (var i = 0; i < colorResIds.Length; i++)
            {
                colorRes[i] = ContextCompat.GetColor(Context, colorResIds[i]);
            }

            SetColorSchemeColors(colorRes);
        }

        /**
         * Set the colors used in the progress animation. The first
         * color will also be the color of the bar that grows in response to a user
         * swipe gesture.
         *
         * @param colors
         */
        public void SetColorSchemeColors(int[] colors)
        {
            EnsureTarget();
            _progress.SetColorSchemeColors(colors);
        }

        /**
         * @return Whether the SwipeRefreshWidget is actively showing refresh
         *         progress.
         */
        public bool IsRefreshing()
        {
            return _refreshing;
        }

        private void EnsureTarget()
        {
            // Don't bother getting the parent height if the parent hasn't been laid
            // out yet.
            if (_target != null) 
                return;
            
            for (var i = 0; i < ChildCount; i++)
            {
                var child = GetChildAt(i);
                if (!child.GetType().IsAssignableFrom(typeof(RecyclerView))) 
                    continue;
                    
                _target = child;
                break;
            }
        }

        /**
         * Set the distance to trigger a sync in dips
         *
         * @param distance
         */
        public void SetDistanceToTriggerSync(int distance)
        {
            _totalDragDistance = distance;
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            var width = MeasuredWidth;
            var height = MeasuredHeight;
            if (ChildCount == 0)
            {
                return;
            }

            if (_target == null)
            {
                EnsureTarget();
            }

            if (_target == null)
            {
                return;
            }

            var child = _target;
            var childLeft = PaddingLeft;
            var childTop = PaddingTop;
            var childWidth = width - PaddingLeft - PaddingRight;
            var childHeight = height - PaddingTop - PaddingBottom;
            child.Layout(childLeft, childTop, childLeft + childWidth, childTop + childHeight);
            var circleWidth = _circleView.MeasuredWidth;
            var circleHeight = _circleView.MeasuredHeight;
            _circleView.Layout((width / 2 - circleWidth / 2), _currentTargetOffsetTop,
                (width / 2 + circleWidth / 2), _currentTargetOffsetTop + circleHeight);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            if (_target == null)
            {
                EnsureTarget();
            }

            if (_target == null)
            {
                return;
            }

            _target.Measure(MeasureSpec.MakeMeasureSpec(
                MeasuredWidth - PaddingLeft - PaddingRight,
                MeasureSpecMode.Exactly), MeasureSpec.MakeMeasureSpec(
                MeasuredHeight - PaddingTop - PaddingBottom, MeasureSpecMode.Exactly));
            _circleView.Measure(MeasureSpec.MakeMeasureSpec(_circleWidth, MeasureSpecMode.Exactly),
                MeasureSpec.MakeMeasureSpec(_circleHeight, MeasureSpecMode.Exactly));
            if (!_usingCustomStart && !_originalOffsetCalculated)
            {
                _originalOffsetCalculated = true;
                _currentTargetOffsetTop = OriginalOffsetTop = -_circleView.MeasuredHeight;
            }

            _circleViewIndex = -1;
            // Get the index of the circleview.
            for (var index = 0; index < ChildCount; index++)
            {
                if (GetChildAt(index) != _circleView) 
                    continue;
                
                _circleViewIndex = index;
                break;
            }
        }

        /**
         * Get the diameter of the progress circle that is displayed as part of the
         * swipe to refresh layout. This is not valid until a measure pass has
         * completed.
         *
         * @return Diameter in pixels of the progress circle view.
         */
        public int GetProgressCircleDiameter()
        {
            return _circleView?.MeasuredHeight ?? 0;
        }

        /**
         * @return Whether it is possible for the child view of this layout to
         *         scroll up. Override this if the child view is a custom view.
         */
        public bool CanChildScrollUp()
        {
            //        //For make it can work when my recycler view is in Gone.
            //        return false;
            if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
            {
                // 14
                if (_target is AbsListView absListView)
                {
                    return absListView.ChildCount > 0
                           && (absListView.FirstVisiblePosition > 0 || absListView.GetChildAt(0)
                               .Top < absListView.PaddingTop);
                }
                else
                {
                    return ViewCompat.CanScrollVertically(_target, -1) || _target.ScrollY > 0;
                }
            }
            else
            {
                return ViewCompat.CanScrollVertically(_target, -1);
            }
        }

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            return base.DispatchTouchEvent(ev);
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            EnsureTarget();

            var action = ev.Action;
            if (_returningToStart && action == MotionEventActions.Down)
            {
                _returningToStart = false;
            }

            if (!Enabled || _returningToStart || CanChildScrollUp() || _refreshing)
            {
                // Fail fast if we're not in a state where a swipe is possible
                return false;
            }

            switch (action)
            {
                case MotionEventActions.Down:
                    SetTargetOffsetTopAndBottom(OriginalOffsetTop - _circleView.Top, true);
                    _activePointerId = ev.GetPointerId(0);
                    _isBeingDragged = false;
                    var initialDownY = GetMotionEventY(ev, _activePointerId);
                    if (initialDownY == -1)
                    {
                        return false;
                    }

                    _initialDownY = initialDownY;
                    break;

                case MotionEventActions.Move:
                    if (_activePointerId == InvalidPointer)
                    {
                        Log.Error(LogTag, "Got ACTION_MOVE event but don't have an active pointer id.");
                        return false;
                    }

                    var y = GetMotionEventY(ev, _activePointerId);
                    if (y == -1)
                    {
                        return false;
                    }

                    var yDiff = y - _initialDownY;
                    if (yDiff > _touchSlop && !_isBeingDragged)
                    {
                        _initialMotionY = _initialDownY + _touchSlop;
                        _isBeingDragged = true;
                        _progress.SetAlpha(StartingProgressAlpha);
                    }

                    break;

                case MotionEventActions.PointerUp:
                    OnSecondaryPointerUp(ev);
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    _isBeingDragged = false;
                    _activePointerId = InvalidPointer;
                    break;
            }

            return _isBeingDragged;
        }

        private static float GetMotionEventY(MotionEvent ev, int activePointerId)
        {
            var index = ev.FindPointerIndex(activePointerId);
            if (index < 0)
            {
                return -1;
            }

            return ev.GetY(index);
        }

        public override void RequestDisallowInterceptTouchEvent(bool b)
        {
            // Nope.
            //Why Nope?
            base.RequestDisallowInterceptTouchEvent(b);
        }

        private static bool IsAnimationRunning(Animation animation)
        {
            return animation != null && animation.HasStarted && !animation.HasEnded;
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            var action = ev.Action;

            if (_returningToStart && action == MotionEventActions.Down)
            {
                _returningToStart = false;
            }

            if (!Enabled || _returningToStart || CanChildScrollUp())
            {
                // Fail fast if we're not in a state where a swipe is possible
                return false;
            }

            switch (action)
            {
                case MotionEventActions.Down:
                    _activePointerId = ev.GetPointerId(0);
                    _isBeingDragged = false;
                    break;

                case MotionEventActions.Move:
                    var pointerIndex = ev.FindPointerIndex(_activePointerId);
                    if (pointerIndex < 0)
                    {
                        Log.Error(LogTag, "Got ACTION_MOVE event but have an invalid active pointer id.");
                        return false;
                    }

                    var y = ev.GetY(pointerIndex);
                    var overscrollTop = (y - _initialMotionY) * DragRate;
                    if (_isBeingDragged)
                    {
                        _progress.ShowArrow(true);
                        var originalDragPercent = overscrollTop / _totalDragDistance;
                        if (originalDragPercent < 0)
                        {
                            return false;
                        }

                        var dragPercent = System.Math.Min(1f, System.Math.Abs(originalDragPercent));
                        var adjustedPercent = (float) System.Math.Max(dragPercent - .4, 0) * 5 / 3;
                        var extraOS = System.Math.Abs(overscrollTop) - _totalDragDistance;
                        var slingshotDist = _usingCustomStart
                            ? _spinnerFinalOffset
                              - OriginalOffsetTop
                            : _spinnerFinalOffset;
                        var tensionSlingshotPercent = System.Math.Max(0,
                            System.Math.Min(extraOS, slingshotDist * 2) / slingshotDist);
                        var tensionPercent = (float) ((tensionSlingshotPercent / 4) - System.Math.Pow(
                            (tensionSlingshotPercent / 4), 2)) * 2f;
                        var extraMove = (slingshotDist) * tensionPercent * 2;

                        var targetY = OriginalOffsetTop
                                      + (int) ((slingshotDist * dragPercent) + extraMove);
                        // where 1.0f is a full circle
                        if (_circleView.Visibility != ViewStates.Visible)
                        {
                            _circleView.Visibility = ViewStates.Visible;
                        }

                        if (!_scale)
                        {
                            ViewCompat.SetScaleX(_circleView, 1f);
                            ViewCompat.SetScaleY(_circleView, 1f);
                        }

                        if (overscrollTop < _totalDragDistance)
                        {
                            if (_scale)
                            {
                                SetAnimationProgress(overscrollTop / _totalDragDistance);
                            }

                            if (_progress.Alpha > StartingProgressAlpha
                                && !IsAnimationRunning(_alphaStartAnimation))
                            {
                                // Animate the alpha
                                StartProgressAlphaStartAnimation();
                            }

                            var strokeStart = adjustedPercent * .8f;
                            _progress.SetStartEndTrim(0f, System.Math.Min(MaxProgressAngle, strokeStart));
                            _progress.SetArrowScale(System.Math.Min(1f, adjustedPercent));
                        }
                        else
                        {
                            if (_progress.Alpha < MaxAlpha
                                && !IsAnimationRunning(_alphaMaxAnimation))
                            {
                                // Animate the alpha
                                StartProgressAlphaMaxAnimation();
                            }
                        }

                        var rotation = (-0.25f + .4f * adjustedPercent + tensionPercent * 2) * .5f;
                        _progress.SetProgressRotation(rotation);
                        SetTargetOffsetTopAndBottom(targetY - _currentTargetOffsetTop,
                            true /* requires update */);
                    }

                    break;

                //case MotionEventActions.Down: 
                //    int index = ev.ActionIndex;
                //    mActivePointerId = ev.GetPointerId(index);
                //    break;


                case MotionEventActions.PointerUp:
                    OnSecondaryPointerUp(ev);
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                {
                    if (_activePointerId == InvalidPointer)
                    {
                        if (action == MotionEventActions.Up)
                        {
                            Log.Error(LogTag, "Got ACTION_UP event but don't have an active pointer id.");
                        }

                        return false;
                    }

                    var pointerIndex0 = ev.FindPointerIndex(_activePointerId);
                    var y0 = ev.GetY(pointerIndex0);
                    var overscrollTop0 = (y0 - _initialMotionY) * DragRate;
                    _isBeingDragged = false;
                    if (overscrollTop0 > _totalDragDistance)
                    {
                        SetRefreshing(true, true /* notify */);
                    }
                    else
                    {
                        // cancel refresh
                        _refreshing = false;
                        _progress.SetStartEndTrim(0f, 0f);
                        IAnimationListener listener = null;
                        if (!_scale)
                        {
                            listener = new CustomCancelListener(this);
                        }

                        AnimateOffsetToStartPosition(_currentTargetOffsetTop, listener);
                        _progress.ShowArrow(false);
                    }

                    _activePointerId = InvalidPointer;
                    return false;
                }
            }

            return true;
        }

        private void AnimateOffsetToCorrectPosition(int from, IAnimationListener listener)
        {
            From = from;
            _animateToCorrectPosition.Reset();
            _animateToCorrectPosition.Duration = (AnimateToTriggerDuration);
            _animateToCorrectPosition.Interpolator = (_decelerateInterpolator);
            if (listener != null)
            {
                _circleView.SetAnimationListener(listener);
            }

            _circleView.ClearAnimation();
            _circleView.StartAnimation(_animateToCorrectPosition);
        }

        private void AnimateOffsetToStartPosition(int from, IAnimationListener listener)
        {
            if (_scale)
            {
                // Scale the item back down
                StartScaleDownReturnToStartAnimation(from, listener);
            }
            else
            {
                From = from;
                _animateToStartPosition.Reset();
                _animateToStartPosition.Duration = (AnimateToStartDuration);
                _animateToStartPosition.Interpolator = (_decelerateInterpolator);
                if (listener != null)
                {
                    _circleView.SetAnimationListener(listener);
                }

                _circleView.ClearAnimation();
                _circleView.StartAnimation(_animateToStartPosition);
            }
        }

        private readonly Animation _animateToCorrectPosition;

        private void MoveToStart(float interpolatedTime)
        {
            var targetTop = 0;
            targetTop = From + (int) ((OriginalOffsetTop - From) * interpolatedTime);
            var offset = targetTop - _circleView.Top;
            SetTargetOffsetTopAndBottom(offset, false /* requires update */);
        }

        private readonly Animation _animateToStartPosition;

        private void StartScaleDownReturnToStartAnimation(int from,
            IAnimationListener listener)
        {
            From = from;
            if (IsAlphaUsedForScale())
            {
                _startingScale = _progress.Alpha;
            }
            else
            {

                _startingScale = ViewCompat.GetScaleX(_circleView);
            }

            _scaleDownToStartAnimation = new CustommScaleDownToStartAnimation(this)
            {
                Duration = ScaleDownDuration
            };
            
            if (listener != null)
            {
                _circleView.SetAnimationListener(listener);
            }

            _circleView.ClearAnimation();
            _circleView.StartAnimation(_scaleDownToStartAnimation);
        }

        private void SetTargetOffsetTopAndBottom(int offset, bool requiresUpdate)
        {
            _circleView.BringToFront();
            _circleView.OffsetTopAndBottom(offset);
            _currentTargetOffsetTop = _circleView.Top;
            if (requiresUpdate && Build.VERSION.SdkInt < BuildVersionCodes.Honeycomb)
            {
                Invalidate();
            }
        }

        private void OnSecondaryPointerUp(MotionEvent ev)
        {
            var pointerIndex = ev.ActionIndex;
            var pointerId = ev.GetPointerId(pointerIndex);
            
            if (pointerId != _activePointerId) 
                return;
            // This was our active pointer going up. Choose a new
            // active pointer and adjust accordingly.
            var newPointerIndex = pointerIndex == 0 ? 1 : 0;
            _activePointerId = ev.GetPointerId(newPointerIndex);
        }

        private class CustomRefreshListener : Java.Lang.Object, IAnimationListener
        {
            private readonly SwipeRefreshLayout _swipeRefreshLayout;

            public CustomRefreshListener(SwipeRefreshLayout swipeRefreshLayout)
            {
                _swipeRefreshLayout = swipeRefreshLayout;
            }

            public void OnAnimationStart(Animation animation)
            {
            }

            public void OnAnimationRepeat(Animation animation)
            {
            }

            public void OnAnimationEnd(Animation animation)
            {
                if (_swipeRefreshLayout._refreshing)
                {
                    // Make sure the progress view is fully visible
                    _swipeRefreshLayout._progress.SetAlpha(MaxAlpha);
                    _swipeRefreshLayout._progress.Start();
                    if (_swipeRefreshLayout._notify)
                    {
                        _swipeRefreshLayout._listener?.OnRefresh();
                    }
                }
                else
                {
                    _swipeRefreshLayout._progress.Stop();
                    _swipeRefreshLayout._circleView.Visibility = ViewStates.Gone;
                    _swipeRefreshLayout.SetColorViewAlpha(MaxAlpha);
                    // Return the circle to its start position
                    if (_swipeRefreshLayout._scale)
                    {
                        _swipeRefreshLayout.SetAnimationProgress(0 /* animation complete and view is hidden */);
                    }
                    else
                    {
                        _swipeRefreshLayout.SetTargetOffsetTopAndBottom(
                            _swipeRefreshLayout.OriginalOffsetTop - _swipeRefreshLayout._currentTargetOffsetTop,
                            true /* requires update */);
                    }
                }

                _swipeRefreshLayout._currentTargetOffsetTop = _swipeRefreshLayout._circleView.Top;
            }
        }

        private class CustomScaleAnimation : Animation
        {
            private readonly SwipeRefreshLayout _swipeRefreshLayout;

            public CustomScaleAnimation(SwipeRefreshLayout swipeRefreshLayout)
            {
                _swipeRefreshLayout = swipeRefreshLayout;
            }

            protected override void ApplyTransformation(float interpolatedTime, Transformation t)
            {
                _swipeRefreshLayout.SetAnimationProgress(interpolatedTime);
            }
        }

        private class CustomAlphaAnimation : Animation
        {
            private readonly SwipeRefreshLayout _swipeRefreshLayout;
            private readonly int _startingAlpha;
            private readonly int _endingAlpha;

            public CustomAlphaAnimation(SwipeRefreshLayout swipeRefreshLayout, int startingAlpha, int endingAlpha)
            {
                _swipeRefreshLayout = swipeRefreshLayout;
                _startingAlpha = startingAlpha;
                _endingAlpha = endingAlpha;
            }

            protected override void ApplyTransformation(float interpolatedTime, Transformation t)
            {
                _swipeRefreshLayout._progress.SetAlpha((int) (_startingAlpha + (_endingAlpha - _startingAlpha)
                    * interpolatedTime));
            }
        }

        private class CustomCancelListener : Object, IAnimationListener
        {
            private readonly SwipeRefreshLayout _swipeRefreshLayout;

            public CustomCancelListener(SwipeRefreshLayout swipeRefreshLayout)
            {
                _swipeRefreshLayout = swipeRefreshLayout;
            }

            public void OnAnimationStart(Animation animation)
            {
            }

            public void OnAnimationRepeat(Animation animation)
            {
            }

            public void OnAnimationEnd(Animation animation)
            {
                if (!_swipeRefreshLayout._scale)
                {
                    _swipeRefreshLayout.StartScaleDownAnimation(null);
                }
            }
        }

        private class CustommAnimateToCorrectPosition : Animation
        {
            private readonly SwipeRefreshLayout _swipeRefreshLayout;

            public CustommAnimateToCorrectPosition(SwipeRefreshLayout swipeRefreshLayout)
            {
                _swipeRefreshLayout = swipeRefreshLayout;
            }

            protected override void ApplyTransformation(float interpolatedTime, Transformation t)
            {
                var targetTop = 0;
                var endTarget = 0;
                if (!_swipeRefreshLayout._usingCustomStart)
                {
                    endTarget = (int) (_swipeRefreshLayout._spinnerFinalOffset -
                                       System.Math.Abs(_swipeRefreshLayout.OriginalOffsetTop));
                }
                else
                {
                    endTarget = (int) _swipeRefreshLayout._spinnerFinalOffset;
                }

                targetTop = _swipeRefreshLayout.From +
                            (int) ((endTarget - _swipeRefreshLayout.From) * interpolatedTime);
                var offset = targetTop - _swipeRefreshLayout._circleView.Top;
                _swipeRefreshLayout.SetTargetOffsetTopAndBottom(offset, false /* requires update */);
                _swipeRefreshLayout._progress.SetArrowScale(1 - interpolatedTime);
            }
        }

        private class CustommAnimateToStartPosition : Animation
        {
            private readonly SwipeRefreshLayout _swipeRefreshLayout;

            public CustommAnimateToStartPosition(SwipeRefreshLayout swipeRefreshLayout)
            {
                _swipeRefreshLayout = swipeRefreshLayout;
            }

            protected override void ApplyTransformation(float interpolatedTime, Transformation t)
            {
                _swipeRefreshLayout.MoveToStart(interpolatedTime);
            }
        }

        private class CustommScaleDownToStartAnimation : Animation
        {
            private readonly SwipeRefreshLayout _swipeRefreshLayout;

            public CustommScaleDownToStartAnimation(SwipeRefreshLayout swipeRefreshLayout)
            {
                _swipeRefreshLayout = swipeRefreshLayout;
            }

            protected override void ApplyTransformation(float interpolatedTime, Transformation t)
            {
                var targetScale = _swipeRefreshLayout._startingScale +
                                  -_swipeRefreshLayout._startingScale * interpolatedTime;
                _swipeRefreshLayout.SetAnimationProgress(targetScale);
                _swipeRefreshLayout.MoveToStart(interpolatedTime);
            }
        }

        private class CustommScaleDownAnimation : Animation
        {
            private readonly SwipeRefreshLayout _swipeRefreshLayout;

            public CustommScaleDownAnimation(SwipeRefreshLayout swipeRefreshLayout)
            {
                _swipeRefreshLayout = swipeRefreshLayout;
            }

            protected override void ApplyTransformation(float interpolatedTime, Transformation t)
            {
                _swipeRefreshLayout.SetAnimationProgress(1 - interpolatedTime);
            }
        }
    }
#pragma warning restore 618
}