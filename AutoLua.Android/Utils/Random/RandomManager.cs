using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace AutoLua.Droid.Utils.Random
{
    /// <summary>
    /// 乱数生成器。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public sealed class RandomManager
    {
        private static readonly RNGCryptoServiceProvider RngCryptoServiceProvider = new RNGCryptoServiceProvider();
        private static readonly byte[] RngBytes = new byte[4];

        /// <summary>
        /// 产生一个非负数的乱数。
        /// </summary>
        /// <returns>随机数。</returns>
        public static int CreateRandomNumber()
        {
            RngCryptoServiceProvider.GetBytes(RngBytes);
            var value = BitConverter.ToInt32(RngBytes, 0);
            if (value < 0)
            {
                value = -value;
            }

            return value;
        }

        /// <summary>
        /// 产生一个非负数且最大值在 max 以下的乱数。
        /// </summary>
        /// <param name="max">最大值。</param>
        /// <returns>随机数。</returns>
        public static int CreateRandomNumber(int max)
        {
            RngCryptoServiceProvider.GetBytes(RngBytes);
            var value = BitConverter.ToInt32(RngBytes, 0);
            value %= (max + 1);
            if (value < 0)
            {
                value = -value;
            }

            return value;
        }

        /// <summary>
        /// 产生一个非负数且最大值在 max 以下的乱数。
        /// </summary>
        /// <param name="max">最大值。</param>
        /// <returns>随机数。</returns>
        public static double CreateRandomNumber(double max)
        {
            RngCryptoServiceProvider.GetBytes(RngBytes);
            var value = BitConverter.ToInt32(RngBytes, 0);
            var c = value % (max + 1);
            if (c < 0)
            {
                c = -c;
            }

            return c;
        }

        /// <summary>
        /// 产生一个非负数且最小值在 min 以上最大值在 max 以下的乱数。
        /// </summary>
        /// <param name="min">最小值（包含）。</param>
        /// <param name="max">最大值（不包含）。</param>
        /// <returns>随机数。</returns>
        public static int CreateRandomNumber(int min, int max)
        {
            if (min == max)
            {
                return max;
            }

            if (max < min)
            {
                return 0;
            }

            max--;

            int value = CreateRandomNumber(max - min) + min;

            return value;
        }

        /// <summary>
        /// 产生一个非负数且最小值在 min 以上最大值在 max 以下的乱数。
        /// </summary>
        /// <param name="min">最小值（包含）。</param>
        /// <param name="max">最大值（不包含）。</param>
        /// <returns>随机数。</returns>
        public static double CreateRandomNumber(double min, double max)
        {
            if (Math.Abs(min - max) < 0)
            {
                return max;
            }

            if (max < min)
            {
                return 0;
            }

            max--;

            var value = CreateRandomNumber(max - min) + min;

            value = Math.Round(value, 2);

            return value;
        }

        /// <summary>
        /// 创建一个指定范围内的随机数字，并降低最小值出现的机率。
        /// </summary>
        /// <param name="min">最小值（包含）。</param>
        /// <param name="max">最大值（不包含）。</param>
        /// <returns>随机数。</returns>
        public static int CreateRandomNumberLowMin(int min, int max)
        {
            if (min == max)
            {
                return max;
            }

            if (max < min)
            {
                return 0;
            }

            var container = new List<int>();

            for (var i = min; i <= max; i++)
            {
                if (i == min)
                {
                    container.Add(i);
                }
                else
                {
                    container.Add(i);
                    container.Add(i);
                }
            }

            var index = CreateRandomNumber(0, container.Count - 1);

            return container[index];
        }

        /// <summary>
        /// 创建一个指定范围内的随机数字，并降低最小值出现的机率。
        /// </summary>
        /// <param name="min">最小值（包含）。</param>
        /// <param name="max">最大值（不包含）。</param>
        /// <returns>随机数。</returns>
        public static double CreateRandomNumberLowMin(double min, double max)
        {
            if (max < min)
            {
                return 0;
            }

            max--;

            var value = CreateRandomNumber(max - min) + min;

            value = Math.Round(value, 2);

            return value;
        }

        /// <summary>
        /// 创建一个指定范围内的随机数字，并降低最小值出现的机率。
        /// </summary>
        /// <param name="min">最小值（包含）。</param>
        /// <param name="max">最大值（不包含）。</param>
        /// <param name="isInterval">是否通过累加0.1的方式增加区间长度。</param>
        /// <returns>随机数。</returns>
        public static double CreateRandomNumberLowMin(double min, double max, bool isInterval = false)
        {
            if (min == max)
            {
                return max;
            }

            if (max < min)
            {
                return 0;
            }

            var container = isInterval ? RandomIntervalDoubles(min, max) : RandomInterval(min, max);

            if (!container.Any())
            {
                return 0;
            }

            var index = CreateRandomNumber(0, container.Count - 1);

            return container[index];
        }

        private static List<double> RandomInterval(double min, double max)
        {
            var container = new List<double>();

            for (var i = min; i <= max; i++)
            {
                if (i == min)
                {
                    container.Add(i);
                }
                else
                {
                    container.Add(i);
                    container.Add(i);
                }
            }

            return container;
        }

        private static List<double> RandomIntervalDoubles(double min, double max)
        {
            var container = new List<double>();

            for (var i = min; i <= max; i += 0.1)
            {
                if (i == min)
                {
                    container.Add(i);
                }
                else
                {
                    container.Add(i);
                    container.Add(i);
                }
            }

            return container;
        }
    }
}