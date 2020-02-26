using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Display;
using Android.Media;
using Android.Media.Projection;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AutoLua.Droid.LuaScript.Api;
using AutoLua.Droid.Utils;
using static Android.Media.ImageReader;
using Image = Android.Media.Image;

namespace AutoLua.Droid.LuaScript.Utils.ScreenCaptures
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public abstract class ScreenCapturerServerBase : IDisposable
    {
        protected static string Tag = "ScreenCapturerServerBase";

        private static MediaProjectionManager _mediaProjectionManager;
        private static MediaProjection _mediaProjection;
        private static VirtualDisplay _virtualDisplay;

        private static volatile ImageReader _imageReader;
        private static volatile bool IsCapture = false;
        private static volatile VolatileDispose volatileDispose;

        private static Intent _intent;
        private static Context Context;

        private static OrientationEventListener _orientationEventListener;

        private static int ImageCacheNum = 1;

        private static bool IsInit = false;
        private static readonly object Lock = new object();

        public abstract string GetTag();

        protected ScreenCapturerServerBase()
        {
            Tag = GetTag();
        }

        /// <summary>
        /// 初始化截屏服务。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <param name="handler"></param>
        public void Init(Intent data, Context context)
        {
            if (IsInit)
                return;

            volatileDispose = new VolatileDispose();
            _intent = data;
            Context = context;
            _mediaProjectionManager = AppApplication.GetSystemService<MediaProjectionManager>(Context.MediaProjectionService);
            _orientationEventListener = new OrientationEvent(context, i =>
            {
                try
                {
                    RefreshVirtualDisplay();
                }
                catch (Exception)
                {
                }
            });

            if (_orientationEventListener.CanDetectOrientation())
                _orientationEventListener.Enable();

            IsInit = true;
        }


        public void SetMaxImageCache(int num)
        {
            if (num <= 0)
                num = 2;

            ImageCacheNum = num;
        }

        /// <summary>
        /// 截屏。
        /// </summary>
        /// <returns></returns>
        public void Capture(VolatileDispose @volatile)
        {
            volatileDispose = @volatile;

            lock (Lock)
            {
                IsCapture = true;
            }
        }

        /// <summary>
        /// 刷新虚拟显示
        /// </summary>
        /// <param name="orientation"></param>
        private static void RefreshVirtualDisplay()
        {
            _imageReader?.Close();
            _virtualDisplay?.Release();
            _mediaProjection?.Stop();

            _mediaProjection = _mediaProjectionManager.GetMediaProjection((int)Result.Ok, _intent.Clone().JavaCast<Intent>());

            //屏幕方向
            var orientation = Context.Resources.Configuration.Orientation;

            //屏幕宽高
            var screenHeight = ScreenMetrics.Instance.GetOrientationAwareScreenHeight(orientation);
            var screenWidth = ScreenMetrics.Instance.GetOrientationAwareScreenWidth(orientation);

            InitVirtualDisplay(screenWidth, screenHeight, (int)ScreenMetrics.Instance.DeviceScreenDensity);
            SetImageListener(new Handler());
        }

        /// <summary>
        /// 初始化截屏
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="dpi"></param>
        private static void InitVirtualDisplay(int width, int height, int dpi)
        {
            var rgbx = (ImageFormatType)Format.Rgbx8888;

            _imageReader = ImageReader.NewInstance(width, height, rgbx, ImageCacheNum);

            _virtualDisplay = _mediaProjection.CreateVirtualDisplay(Tag, width, height, dpi, DisplayFlags.Round, _imageReader.Surface, null, null);
        }

        private static void SetImageListener(Handler handler)
        {
            _imageReader.SetOnImageAvailableListener(new OnImageAvailableListener(read =>
            {
                try
                {
                    if (!IsCapture)
                        return;

                    var image = read.AcquireLatestImage();
                    var bitmap = ToBitmap(image);
                    //image.Close();
                    IsCapture = false;
                    volatileDispose.setAndNotify(bitmap);
                }
                catch (Exception)
                {
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
            int pixelStride = plane.PixelStride;
            int rowPadding = plane.RowStride - pixelStride * image.Width;

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

        private class ImageAvailableListener : Java.Lang.Object, IOnImageAvailableListener
        {
            private readonly Action<ImageReader> action;

            public ImageAvailableListener(Action<ImageReader> action)
            {
                this.action = action;
            }

            public void OnImageAvailable(ImageReader reader)
            {
                action?.Invoke(reader);
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

        private class OnImageAvailableListener : Java.Lang.Object, IOnImageAvailableListener
        {
            private readonly Action<ImageReader> action;

            public OnImageAvailableListener(Action<ImageReader> action)
            {
                this.action = action;
            }

            public void OnImageAvailable(ImageReader reader)
            {
                action?.Invoke(reader);
            }
        }
    }
}