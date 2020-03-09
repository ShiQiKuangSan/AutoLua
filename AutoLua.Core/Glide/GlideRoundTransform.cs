using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Com.Bumptech.Glide.Load.Engine.Bitmap_recycle;
using Com.Bumptech.Glide.Load.Resource.Bitmap;

namespace AutoLua.Core.Glide
{
    public class GlideRoundTransform : BitmapTransformation
    {
        private static float _radius = 0f;

        public GlideRoundTransform(Context context, int dp = 4)
            : base(context)
        {
            _radius = Resources.System.DisplayMetrics.Density * dp;
        }

        public override string Id => Class.Name + Math.Round(_radius);

        protected override Bitmap Transform(IBitmapPool pool, Bitmap toTransform,
            int outWidth, int outHeight)
        {
            return RoundCrop(pool, toTransform);
        }

        private static Bitmap RoundCrop(IBitmapPool pool, Bitmap source)
        {
            if (source == null) return null;
            var result = pool.Get(source.Width, source.Height, Bitmap.Config.Argb8888);

            if (result == null)
            {
                result = Bitmap.CreateBitmap(source.Width, source.Height, Bitmap.Config.Argb8888);
            }

            var canvas = new Canvas(result);
            var paint = new Paint();

            paint.SetShader(new BitmapShader(source, Shader.TileMode.Clamp, Shader.TileMode.Clamp));
            paint.AntiAlias = true;

            var rectF = new RectF(0f, 0f, source.Width, source.Height);
            canvas.DrawRoundRect(rectF, _radius, _radius, paint);
            return result;
        }
    }
}