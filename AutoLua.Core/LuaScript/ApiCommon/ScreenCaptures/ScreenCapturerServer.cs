using System;
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
using AutoLua.Core.Common;
using AutoLua.Core.LuaScript.Api;
using static Android.Media.ImageReader;
using Image = Android.Media.Image;

namespace AutoLua.Core.LuaScript.ApiCommon.ScreenCaptures
{
    [Preserve(AllMembers = true)]
    public sealed class ScreenCapturerServer : Java.Lang.Object, IOnImageAvailableListener
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
        private volatile Bitmap _bitmap = null;

        private Handler _handler = new Handler();
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
            _isInit = true;
        }

        /// <summary>
        /// 截屏。
        /// </summary>
        /// <returns></returns>
        public void Capture()
        {
            StartVirtualDisplay();
        }

        public Bitmap GetBitmap()
        {
            var t = new Task(() =>
            {
                var i = 60;

                while (_bitmap == null && i > 0)
                {
                    Task.Delay(200);
                    i--;
                }
            });

            t.Start();

            t.Wait();

            return _bitmap;
        }

        /// <summary>
        /// 刷新虚拟显示
        /// </summary>
        private void StartVirtualDisplay()
        {
            _imageReader?.Close();
            _virtualDisplay?.Release();
            _mediaProjection?.Stop();

            _mediaProjection = _mediaProjectionManager.GetMediaProjection((int)Result.Ok, _intent);

            //屏幕宽高
            var screenHeight = ScreenMetrics.Instance.GetOrientationAwareScreenHeight();
            var screenWidth = ScreenMetrics.Instance.GetOrientationAwareScreenWidth();

            InitVirtualDisplay(screenWidth, screenHeight, (int)ScreenMetrics.Instance.DeviceScreenDensity);
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

            _imageReader.SetOnImageAvailableListener(this, _handler);
        }

        /// <summary>
        /// 转换数据
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private Bitmap ToBitmap(Image image)
        {
            var plane = image.GetPlanes()[0];
            var pixelStride = plane.PixelStride;
            var rowPadding = plane.RowStride - pixelStride * image.Width;
            var buffer = plane.Buffer;

            var bitmap = Bitmap.CreateBitmap(image.Width + (rowPadding / pixelStride), image.Height, Bitmap.Config.Argb8888);

            bitmap.CopyPixelsFromBuffer(buffer);
            _bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, image.Width, image.Height);

            image.Close();

            return _bitmap;
        }

        /// <summary>
        /// 释放截图。
        /// </summary>
        public void TearDownMediaProjection()
        {
            _mediaProjection?.Stop();
            _mediaProjection = null;
            _virtualDisplay?.Release();
            _imageReader?.Close();
            _imageReader = null;
        }

        /// <summary>
        /// 开始截图
        /// </summary>
        /// <param name="reader"></param>
        public void OnImageAvailable(ImageReader reader)
        {
            var image = reader.AcquireNextImage();
            _bitmap = ToBitmap(image);
            TearDownMediaProjection();
        }
    }
}