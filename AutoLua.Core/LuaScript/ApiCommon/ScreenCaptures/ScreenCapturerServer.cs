using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Display;
using Android.Media;
using Android.Media.Projection;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AutoLua.Core.Common;
using AutoLua.Core.LuaScript.Api;
using static Android.Media.ImageReader;
using Image = Android.Media.Image;

namespace AutoLua.Core.LuaScript.ApiCommon.ScreenCaptures
{
    [Preserve(AllMembers = true)]
    public sealed class ScreenCapturerServer : IDisposable
    {
        private const string Tag = "ScreenCapturerServer";

        private MediaProjectionManager _mediaProjectionManager;
        private MediaProjection _mediaProjection;
        private VirtualDisplay _virtualDisplay;

        private ImageReader _imageReader;

        private Intent _intent;
        private Context _context;

        /// <summary>
        /// 是否截屏
        /// </summary>
        private volatile bool _isCapture;
        private volatile VolatileDispose _volatileDispose;
        private Handler _handler;
        private OrientationEventListener _orientationEventListener;

        private const int ImageCacheNum = 1;

        private bool _isInit;
        private static readonly object Lock = new object();

        private static ScreenCapturerServer _instance;

        public static ScreenCapturerServer Instance
        {
            get
            {
                lock (Lock)
                {
                    _instance ??= new ScreenCapturerServer();

                    return _instance;
                }
            }
        }

        /// <summary>
        /// 初始化截屏服务。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        public void Init(Intent data, Context context)
        {
            if (_isInit)
                return;

            _intent = data;
            _context = context;
            _handler = new Handler();
            _mediaProjectionManager = AppUtils.GetSystemService<MediaProjectionManager>(Context.MediaProjectionService);
            _orientationEventListener = new OrientationEvent(context, i =>
            {
                try
                {
                    RefreshVirtualDisplay();
                }
                catch (Exception)
                {
                    // ignored
                }
            });

            if (_orientationEventListener.CanDetectOrientation())
                _orientationEventListener.Enable();


            RefreshVirtualDisplay();

            _isInit = true;
        }

        /// <summary>
        /// 截屏。
        /// </summary>
        /// <returns></returns>
        public void Capture(VolatileDispose volatileDispose)
        {
            _volatileDispose = volatileDispose;
            _isCapture = true;
        }

        /// <summary>
        /// 刷新虚拟显示
        /// </summary>
        private void RefreshVirtualDisplay()
        {
            _imageReader?.Close();
            _virtualDisplay?.Release();
            _mediaProjection?.Stop();

            _mediaProjection = _mediaProjectionManager.GetMediaProjection((int)Result.Ok, _intent);

            //屏幕方向
            var orientation = _context.Resources.Configuration.Orientation;

            //屏幕宽高
            var screenHeight = ScreenMetrics.Instance.GetOrientationAwareScreenHeight(orientation);
            var screenWidth = ScreenMetrics.Instance.GetOrientationAwareScreenWidth(orientation);

            InitVirtualDisplay(screenWidth, screenHeight, (int)ScreenMetrics.Instance.DeviceScreenDensity);
            SetImageListener(_handler);
        }

        /// <summary>
        /// 初始化截屏
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="dpi"></param>
        private void InitVirtualDisplay(int width, int height, int dpi)
        {
            var rgbx = (ImageFormatType)Format.Rgba8888;

            _imageReader = NewInstance(width, height, rgbx, ImageCacheNum);

            _virtualDisplay = _mediaProjection.CreateVirtualDisplay(Tag, width, height, dpi, DisplayFlags.Round, _imageReader.Surface, null, null);
        }


        private void SetImageListener(Handler handler)
        {
            _imageReader.SetOnImageAvailableListener(new ImageAvailableListener((read) =>
            {
                try
                {
                    var image = read.AcquireLatestImage();

                    if (_isCapture && image != null)
                    {
                        var bitmap = ToBitmap(image);
                        image.Close();
                        _isCapture = false;
                        _volatileDispose.setAndNotify(bitmap);
                    }
                    else
                    {
                        image?.Close();
                    }
                }
                catch (Exception e)
                {
                    // ignored
                    var msg = e.Message;
                }
            }), handler);
        }

        /// <summary>
        /// 转换数据
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static Bitmap ToBitmap(Image image)
        {
            var plane = image.GetPlanes()[0];
            var pixelStride = plane.PixelStride;
            var rowPadding = plane.RowStride - pixelStride * image.Width;

            var buffer = plane.Buffer;
            buffer.Position(0);

            var bitmap = Bitmap.CreateBitmap(image.Width + (rowPadding / pixelStride), image.Height, Bitmap.Config.Argb8888);

            bitmap.CopyPixelsFromBuffer(buffer);

            if (rowPadding == 0)
            {
                return bitmap;
            }

            return Bitmap.CreateBitmap(bitmap, 0, 0, image.Width, image.Height);
        }

        public void Dispose()
        {
            _mediaProjection?.Stop();
            _mediaProjection = null;
            _virtualDisplay?.Release();
            _imageReader?.Close();
            _orientationEventListener?.Dispose();
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


        private class ImageAvailableListener : Java.Lang.Object, IOnImageAvailableListener
        {
            private readonly Action<ImageReader> _action;

            public ImageAvailableListener(Action<ImageReader> action)
            {
                _action = action;
            }

            public void OnImageAvailable(ImageReader reader)
            {
                _action.Invoke(reader);
            }
        }
    }
}