using Xamarin.Forms;

namespace AutoLua.Core.LuaScript.Api
{
    /// <summary>
    /// 颜色操作类。
    /// </summary>
    public class Colors
    {
        /// <summary>
        /// 返回颜色color的R通道的值，范围0~255.
        /// </summary>
        /// <param name="color">颜色值</param>
        /// <returns></returns>
        public double r(string color)
        {
            return getColor(color).R;
        }

        /// <summary>
        /// 返回颜色color的G通道的值，范围0~255.
        /// </summary>
        /// <param name="color">颜色值</param>
        /// <returns></returns>
        public double g(string color)
        {
            return getColor(color).G;
        }

        /// <summary>
        /// 返回颜色color的B通道的值，范围0~255.
        /// </summary>
        /// <param name="color">颜色值</param>
        /// <returns></returns>
        public double b(string color)
        {
            return getColor(color).B;
        }

        /// <summary>
        /// 返回颜色color的Alpha通道的值，范围0~255.
        /// </summary>
        /// <param name="color">颜色值</param>
        /// <returns></returns>
        public double a(string color)
        {
            return getColor(color).A;
        }

        /// <summary>
        /// 返回这些颜色通道构成的整数颜色值。Alpha通道将是255（不透明）。
        /// </summary>
        /// <param name="r">颜色的R通道的值</param>
        /// <param name="g">颜色的G通道的值</param>
        /// <param name="b">颜色的B通道的值</param>
        /// <returns></returns>
        public string rgb(int r, int g, int b)
        {
            return Color.FromRgb(r, g, b).ToHex();
        }

        /// <summary>
        /// 返回这些颜色通道构成的整数颜色值。
        /// </summary>
        /// <param name="a">颜色的Alpha通道的值</param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public string argb(int a, int r, int g, int b)
        {
            return Color.FromRgba(r, g, b, a).ToHex();
        }

        /// <summary>
        /// 返回两个颜色是否相等。
        /// </summary>
        /// <param name="c1">颜色值1</param>
        /// <param name="c2">颜色值2</param>
        /// <returns></returns>
        public bool equals(string c1, string c2)
        {
            return Color.FromHex(c1) == Color.FromHex(c2);
        }

        /// <summary>
        /// 获得颜色。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public Color getColor(string color)
        {
            return Color.FromHex(color);
        }
    }
}