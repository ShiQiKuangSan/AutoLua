using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Media;
using Android.Media.Projection;
using Android.OS;
using Android.Util;
using Android.Views;
using AutoLua.Droid.Http;
using AutoLua.Droid.Http.Models;
using AutoLua.Droid.LuaScript.Utils.ScreenCaptures;
using AutoLua.Droid.Utils;
using AutoLua.Droid.Utils.App;
using Java.Lang;
using OpenCvSharp;
using static AutoLua.Droid.LuaScript.Api.LuaFiles;
using Bitmap = Android.Graphics.Bitmap;
using Exception = System.Exception;
using Math = System.Math;
using Pattern = Java.Util.Regex.Pattern;

namespace AutoLua.Droid.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class Images
    {
        public Images()
        {
        }

        /// <summary>
        /// 读取在路径path的图片文件并返回一个Image对象。如果文件不存在或者文件无法解码则返回null。
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <returns></returns>
        public ImageWrapper read(string path)
        {
            path = PFiles.Path(path);
            var bitmap = BitmapFactory.DecodeFile(path);
            return ImageWrapper.OfBitmap(bitmap);
        }

        /// <summary>
        /// 加载在地址URL的网络图片并返回一个Image对象。如果地址不存在或者图片无法解码则返回null。
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ImageWrapper load(string url)
        {
            try
            {
                var uri = new Uri(url);

                var res = HttpClientManager.Current.GetHtml(new HttpItem
                {
                    Url = uri.ToString(),
                    ResultType = ResultType.Byte,
                });

                var bitmap = BitmapFactory.DecodeStream(new MemoryStream(res.ResultByte));

                return ImageWrapper.OfBitmap(bitmap);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 复制一张图片并返回新的副本。该函数会完全复制img对象的数据。
        /// </summary>
        /// <param name="img">图片</param>
        /// <returns></returns>
        public ImageWrapper copy(ImageWrapper img)
        {
            return img.copy();
        }

        /// <summary>
        /// 把图片image以PNG格式保存到path中。如果文件不存在会被创建；文件存在会被覆盖。
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="path">路径</param>
        /// <param name="format">图片格式，可选的值为：png,jpeg/jpg,webp</param>
        /// <param name="quality">图片质量，为0~100的整数值</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool save(ImageWrapper image, string path, string format, int quality)
        {
            var compressFormat = ParseImageFormat(format);

            if (compressFormat == null)
                throw new Exception("unknown format " + format);

            var outputStream = new FileStream(PFiles.Path(path), FileMode.CreateNew);
            return image.Bitmap.Compress(compressFormat, quality, outputStream);
        }


        /// <summary>
        /// 解码Base64数据并返回解码后的图片Image对象。如果base64无法解码则返回null。
        /// </summary>
        /// <param name="data">图片的Base64数据</param>
        /// <returns></returns>
        public ImageWrapper fromBase64(string data)
        {
            var dataPattern = Pattern.Compile("data:(\\w+/\\w+);base64,(.+)");
            var loadBase64Data = new Func<string, Bitmap>((input) =>
            {
                var matcher = dataPattern.Matcher(input);
                string base64;
                if (!matcher.Matches() || matcher.GroupCount() != 2)
                    base64 = input;
                else
                {
                    base64 = matcher.Group(2);
                }

                var bytes = Base64.Decode(base64, Base64Flags.Default);
                return BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
            });

            return ImageWrapper.OfBitmap(loadBase64Data(data));
        }


        /// <summary>
        /// 把图片编码为base64数据并返回。
        /// </summary>
        /// <param name="wrapper">图片</param>
        /// <param name="format">图片格式，可选的值为：png,jpeg/jpg,webp</param>
        /// <param name="quality">图片质量，为0~100的整数值</param>
        /// <returns></returns>
        public string toBase64(ImageWrapper wrapper, string format, int quality)
        {
            return Base64.EncodeToString(toBytes(wrapper, format, quality), Base64Flags.NoWrap);
        }

        /// <summary>
        /// 解码字节数组bytes并返回解码后的图片Image对象。如果bytes无法解码则返回null。
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public ImageWrapper fromBytes(byte[] bytes)
        {
            return ImageWrapper.OfBitmap(BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length));
        }

        /// <summary>
        /// 把图片编码为字节数组并返回。
        /// </summary>
        /// <param name="wrapper">图片</param>
        /// <param name="format">图片格式，可选的值为：png,jpeg/jpg,webp</param>
        /// <param name="quality">图片质量，为0~100的整数值</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte[] toBytes(ImageWrapper wrapper, string format, int quality)
        {
            var compressFormat = ParseImageFormat(format);

            if (compressFormat == null)
                throw new Exception("unknown format " + format);

            var outputStream = new MemoryStream();
            wrapper.Bitmap.Compress(compressFormat, quality, outputStream);

            var imageBytes = new byte[outputStream.Length];

            outputStream.Read(imageBytes, 0, imageBytes.Length);

            outputStream.Close();

            return imageBytes;
        }

        /// <summary>
        /// 从图片img的位置(x, y)处剪切大小为w * h的区域，并返回该剪切区域的新图片。
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="x">剪切区域的左上角横坐标</param>
        /// <param name="y">剪切区域的左上角纵坐标</param>
        /// <param name="x2">剪切区域的宽度</param>
        /// <param name="y2">剪切区域的高度</param>
        /// <returns></returns>
        public ImageWrapper clip(ImageWrapper img, int x, int y, int x2, int y2)
        {
            var w = x2 - x;
            var h = y2 - y;
            return ImageWrapper.OfBitmap(Bitmap.CreateBitmap(img.Bitmap, x, y, w, h));
        }


        /// <summary>
        /// 将图片逆时针旋转degress度，返回旋转后的图片对象。
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="x">旋转中心x坐标，默认为图片中点</param>
        /// <param name="y">旋转中心y坐标，默认为图片中点</param>
        /// <param name="degree">旋转角度。</param>
        /// <returns></returns>
        public ImageWrapper rotate(ImageWrapper img, float x, float y, float degree)
        {
            var matrix = new Matrix();
            matrix.PostRotate(degree, x, y);
            return ImageWrapper.OfBitmap(Bitmap.CreateBitmap(img.Bitmap, 0, 0, img.Width, img.Height, matrix, true));
        }

        /// <summary>
        /// 连接两张图片，并返回连接后的图像。如果两张图片大小不一致，小的那张将适当居中。
        /// </summary>
        /// <param name="img1">图片1</param>
        /// <param name="img2">图片2</param>
        /// <param name="direction">连接方向，默认为"RIGHT"，可选的值有：
        ///    LEFT 将图片2接到图片1左边
        ///    RIGHT 将图片2接到图片1右边
        ///    TOP 将图片2接到图片1上边
        ///    BOTTOM 将图片2接到图片1下边 
        /// </param>
        /// <returns></returns>
        public ImageWrapper concat(ImageWrapper img1, ImageWrapper img2, GravityFlags direction)
        {
            int width;
            int height;

            if (direction == GravityFlags.Left || direction == GravityFlags.Top)
            {
                var tmp = img1;
                img1 = img2;
                img2 = tmp;
            }

            if (direction == GravityFlags.Left || direction == GravityFlags.Right)
            {
                width = img1.Width + img2.Width;
                height = Math.Max(img1.Height, img2.Height);
            }
            else
            {
                width = Math.Max(img1.Width, img2.Height);
                height = img1.Height + img2.Height;
            }

            var bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            var canvas = new Canvas();
            var paint = new Paint();

            if (direction != GravityFlags.Left && direction != GravityFlags.Right)
                return ImageWrapper.OfBitmap(bitmap);

            canvas.DrawBitmap(img1.Bitmap, (width - img1.Width) / 2, 0, paint);
            canvas.DrawBitmap(img2.Bitmap, (width - img2.Width) / 2, img1.Height, paint);

            return ImageWrapper.OfBitmap(bitmap);
        }

        public ImageWrapper captureScreen()
        {
            var capture = ScreenCapturerServerManager.Capturer();

            return ImageWrapper.OfBitmap(capture);
        }

        private static Bitmap.CompressFormat ParseImageFormat(string format)
        {
            return format switch
            {
                "png" => Bitmap.CompressFormat.Png,
                "jpeg" => Bitmap.CompressFormat.Jpeg,
                "jpg" => Bitmap.CompressFormat.Jpeg,
                "webp" => Bitmap.CompressFormat.Webp,
                _ => null
            };
        }
    }

    /// <summary>
    /// image图片对象。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ImageWrapper
    {
        public int Width { get; }

        public int Height { get; }

        public Mat Mat { get; }

        public Bitmap Bitmap { get; }

        private ImageWrapper(Mat mat)
        {
            Mat = mat;
        }

        private ImageWrapper(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }

        private ImageWrapper(Bitmap bitmap, Mat mat)
        {
            Mat = mat;
            Bitmap = bitmap;
        }

        protected ImageWrapper(int width, int height)
        {
            Width = width;
            Height = height;
        }


        public ImageWrapper copy()
        {
            EnsureNotRecycled();
            if (Bitmap == null)
                return OfMat(Mat?.Clone() ?? null);

            return Mat == null
                ? OfBitmap(Bitmap?.Copy(Bitmap?.GetConfig(), true))
                : new ImageWrapper(Bitmap?.Copy(Bitmap?.GetConfig(), true), Mat?.Clone());
        }

        public static ImageWrapper OfBitmap(Bitmap bitmap)
        {
            return bitmap == null ? null : new ImageWrapper(bitmap);
        }
        public static ImageWrapper OfBitmap(Image image)
        {
            if (image == null)
            {
                return null;
            }
            return new ImageWrapper(ToBitmap(image));
        }

        public static Bitmap ToBitmap(Image image)
        {
            var plane = image.GetPlanes()[0];
            var buffer = plane.Buffer;
            buffer.Position(0);

            int pixelStride = plane.PixelStride;
            int rowPadding = plane.RowStride - pixelStride * image.Width;
            var bitmap = Bitmap.CreateBitmap(image.Width + (rowPadding / pixelStride), image.Height, Bitmap.Config.Argb8888);

            bitmap.CopyPixelsFromBuffer(buffer);

            if (rowPadding == 0)
            {
                return bitmap;
            }

            return Bitmap.CreateBitmap(bitmap, 0, 0, image.Width, image.Height);
        }

        public static ImageWrapper OfMat(Mat mat)
        {
            return mat == null ? null : new ImageWrapper(mat);
        }

        private void EnsureNotRecycled()
        {
            if (Bitmap == null && Mat == null)
                throw new Exception("image 已回收");
        }

        public override string ToString()
        {
            var str =
                $"Mat : {Mat?.ToString() ?? "null"} Bitmap : {Bitmap?.ToString() ?? "null"} Width : {Width} Height : {Height}";

            return str;
        }
    }
}