using System;
using Android.Content;
using Android.Graphics;
using Com.Bumptech.Glide.Load.Engine.Bitmap_recycle;
using Com.Bumptech.Glide.Load.Resource.Bitmap;

namespace AutoLua.Core.Glide
{
    public class GlideCircleTransform : BitmapTransformation
    {
        public GlideCircleTransform(Context context)
            : base(context)
        {
        }

        public override string Id => Class.Name;

        protected override Bitmap Transform(IBitmapPool pool, Bitmap toTransform,
            int outWidth, int outHeight)
        {
            return CircleCrop(pool, toTransform);
        }

        private static Bitmap CircleCrop(IBitmapPool pool, Bitmap source)
        {
            if (source == null)
                return null;
            var size = Math.Min(source.Width, source.Height);
            var x = (source.Width - size) / 2;
            var y = (source.Height - size) / 2;
            var squared = Bitmap.CreateBitmap(source, x, y, size, size);
            var result = pool.Get(size, size, Bitmap.Config.Argb8888);
            
            if (result == null)
            {
                result = Bitmap.CreateBitmap(size, size, Bitmap.Config.Argb8888);
            }

            var canvas = new Canvas(result);
            var paint = new Paint();
            paint.SetShader(new BitmapShader(squared, Shader.TileMode.Clamp,
                Shader.TileMode.Clamp));
            paint.AntiAlias = true;
            var r = size / 2f;
            canvas.DrawCircle(r, r, r, paint);
            return result;
        }
    }
}