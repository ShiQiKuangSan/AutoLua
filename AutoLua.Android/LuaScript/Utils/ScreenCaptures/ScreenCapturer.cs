using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Display;
using Android.Media;
using Android.Media.Projection;
using Android.OS;
using Android.Util;
using Android.Views;
using AutoLua.Droid.LuaScript.Api;
using AutoLua.Droid.Utils;
using Java.Interop;
using Java.Lang;
using Java.Util.Concurrent.Atomic;
using Exception = System.Exception;
using Orientation = Android.Content.Res.Orientation;

namespace AutoLua.Droid.LuaScript.Utils.ScreenCaptures
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ScreenCapturer : Java.Lang.Object
    {
        private readonly Context _context;

        /// <summary>
        /// 截屏方向
        /// </summary>
        private Orientation _orientation;

        /// <summary>
        /// 检测的方向
        /// </summary>
        private Orientation _detectedOrientation;

        private readonly Handler _handler;

        /// <summary>
        /// 图像获取
        /// </summary>
        private volatile Looper _imageAcquireLooper;

        private volatile AtomicReference _cachedImage = new AtomicReference();

        private volatile Image _underUsingImage;

        /// <summary>
        /// 图片读取
        /// </summary>
        private ImageReader _imageReader;

        /// <summary>
        /// 
        /// </summary>
        private VirtualDisplay _virtualDisplay;

        /// <summary>
        /// 媒体投影
        /// </summary>
        private MediaProjection _mediaProjection;

        private readonly MediaProjectionManager _projectionManager;

        private readonly Intent _data;

        private OrientationEventListener _orientationEventListener;

        /// <summary>
        /// 异常信息
        /// </summary>
        private volatile Exception _exception;

        /// <summary>
        /// 屏幕密度，dpi
        /// </summary>
        private readonly DisplayMetricsDensity _screenDensity;

        private const string Tag = "ScreenCapturer";

        /// <summary>
        /// 屏幕截图
        /// </summary>
        /// <param name="data"></param>
        /// <param name="orientation">屏幕方向</param>
        /// <param name="screenDensity">屏幕密度（dpi）</param>
        /// <param name="handler"></param>
        public ScreenCapturer(Intent data, Orientation orientation, DisplayMetricsDensity screenDensity, Handler handler)
        {
            _context = AppUtils.GetAppContext;
            _handler = handler;
            _data = data;
            _screenDensity = screenDensity;
            _projectionManager = AppApplication.GetSystemService<MediaProjectionManager>(Context.MediaProjectionService);
            _mediaProjection = _projectionManager.GetMediaProjection((int) Result.Ok,_data.Clone().JavaCast<Intent>());
            SetOrientation(orientation);
            ObserveOrientation();
        }

        /// <summary>
        /// 观察方向。
        /// </summary>
        private void ObserveOrientation()
        {
            _orientationEventListener = new OrientationEvent(_context, o =>
            {
                var orientation = _context.Resources.Configuration.Orientation;

                if (_orientation != Orientation.Undefined || _detectedOrientation == orientation)
                    return;

                _detectedOrientation = orientation;

                try
                {
                    RefreshVirtualDisplay(orientation);
                }
                catch (Exception e)
                {
                    _exception = e;
                }
            });

            if (_orientationEventListener.CanDetectOrientation())
                _orientationEventListener.Enable();
        }

        /// <summary>
        /// 设置方向
        /// </summary>
        /// <param name="orientation"></param>
        public void SetOrientation(Orientation orientation)
        {
            if (_orientation == orientation)
                return;
            _orientation = orientation;

            _detectedOrientation = AppUtils.GetAppContext.Resources.Configuration.Orientation;

            var o = _orientation == Orientation.Undefined ? _detectedOrientation : _orientation;
            RefreshVirtualDisplay(o);
        }


        /// <summary>
        /// 截屏
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Image Capture()
        {
            if (_exception != null)
            {
                var e = _exception;
                _exception = null;
                throw new Exception(e.ToString());
            }

            var thread = Thread.CurrentThread();
            while (!thread.IsInterrupted)
            {
                if (!(_cachedImage.GetAndSet(null) is Image cachedImage))
                    continue;

                _underUsingImage?.Close();
                _underUsingImage = cachedImage;
                return cachedImage;
            }

            throw new Exception();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            _imageAcquireLooper?.Quit();
            _mediaProjection?.Stop();
            _mediaProjection = null;
            _virtualDisplay?.Release();
            _imageReader?.Close();
            _underUsingImage?.Close();

            if (_cachedImage.GetAndSet(null) is Image cachedImage)
            {
                cachedImage.Close();
            }

            _orientationEventListener?.Disable();
        }

        /// <summary>
        /// 刷新虚拟显示
        /// </summary>
        /// <param name="orientation"></param>
        private void RefreshVirtualDisplay(Orientation orientation)
        {
            _imageAcquireLooper?.Quit();
            _imageReader?.Close();
            _virtualDisplay?.Release();
            _mediaProjection?.Stop();
            _mediaProjection =
                _projectionManager.GetMediaProjection((int) Result.Ok, (Intent) _data.Clone());

            //屏幕宽高
            var screenHeight = ScreenMetrics.Instance.GetOrientationAwareScreenHeight(orientation);
            var screenWidth = ScreenMetrics.Instance.GetOrientationAwareScreenWidth(orientation);
            InitVirtualDisplay(screenWidth, screenHeight, _screenDensity);
            StartAcquireImageLoop();
        }

        /// <summary>
        /// 初始化虚拟显示
        /// </summary>
        /// <param name="width">屏幕宽</param>
        /// <param name="height">屏幕高</param>
        /// <param name="screenDensity">屏幕密度</param>
        private void InitVirtualDisplay(int width, int height, DisplayMetricsDensity screenDensity)
        {
            _imageReader = ImageReader.NewInstance(width, height, ImageFormatType.FlexRgb888, 1);
            var dpi = (int)screenDensity;
            //VIRTUAL_DISPLAY_FLAG_AUTO_MIRROR
            _virtualDisplay = _mediaProjection.CreateVirtualDisplay(Tag, width, height, dpi,
                DisplayFlags.Round, _imageReader.Surface, null, null);
        }

        /// <summary>
        /// 开始循环获取屏幕图像信息。
        /// </summary>
        private void StartAcquireImageLoop()
        {
            if (_handler != null)
            {
                SetImageListener(_handler);
                return;
            }

            new Thread(() =>
            {
                Looper.Prepare();
                _imageAcquireLooper = Looper.MainLooper;
                SetImageListener(new Handler());
                Looper.Loop();
            }).Start();
        }

        /// <summary>
        /// 设置图片监听
        /// </summary>
        /// <param name="handler"></param>
        private void SetImageListener(Handler handler)
        {
            _imageReader.SetOnImageAvailableListener(new ImageAvailable(reader =>
            {
                try
                {
                    var oldCacheImage = _cachedImage.GetAndSet(null) as Image;
                    oldCacheImage?.Close();
                    _cachedImage.Set(reader.AcquireLatestImage());
                }
                catch (Exception e)
                {
                    _exception = e;
                }
            }), handler);
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        protected override void JavaFinalize()
        {
            try
            {
                Release();
            }
            finally
            {
                base.JavaFinalize();
            }
        }

        /// <summary>
        /// 方向监视事件
        /// </summary>
        private class OrientationEvent : OrientationEventListener
        {
            private readonly Action<int> _onOrientationChanged;

            public OrientationEvent(Context context, Action<int> onOrientationChanged) : base(context)
            {
                _onOrientationChanged = onOrientationChanged;
            }


            public override void OnOrientationChanged(int orientation)
            {
                _onOrientationChanged?.Invoke(orientation);
            }
        }

        private class ImageAvailable : Java.Lang.Object, ImageReader.IOnImageAvailableListener
        {
            private readonly Action<ImageReader> _onImageAvailable;

            public ImageAvailable(Action<ImageReader> onImageAvailable)
            {
                _onImageAvailable = onImageAvailable;
            }

            public void OnImageAvailable(ImageReader reader)
            {
                _onImageAvailable?.Invoke(reader);
            }
        }
    }
}