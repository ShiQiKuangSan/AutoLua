﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoLua.Core.Extensions.Tools.Random;
using AutoLua.Core.Extensions.Tools.Strings;
using Newtonsoft.Json;

namespace AutoLua.Core.Extensions
{
    public static class Extensions
    {
        #region SyncForEach

        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        public static void ForEach(this object[] objs, Action<object> action)
        {
            foreach (var o in objs)
            {
                action(o);
            }
        }

        /// <summary>
        /// 遍历IEnumerable
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        public static void ForEach(this IEnumerable<dynamic> objs, Action<object> action)
        {
            foreach (var o in objs)
            {
                action(o);
            }
        }

        /// <summary>
        /// 遍历集合
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        public static void ForEach(this IList<dynamic> objs, Action<object> action)
        {
            foreach (var o in objs)
            {
                action(o);
            }
        }

        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        public static void ForEach<T>(this T[] objs, Action<T> action)
        {
            foreach (var o in objs)
            {
                action(o);
            }
        }

        /// <summary>
        /// 遍历IEnumerable
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        public static void ForEach<T>(this IEnumerable<T> objs, Action<T> action)
        {
            foreach (var o in objs)
            {
                action(o);
            }
        }

        /// <summary>
        /// 遍历List
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        public static void ForEach<T>(this IList<T> objs, Action<T> action)
        {
            foreach (var o in objs)
            {
                action(o);
            }
        }

        /// <summary>
        /// 遍历数组并返回一个新的List
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this object[] objs, Func<object, T> action)
        {
            return objs.Select(action);
        }

        /// <summary>
        /// 遍历IEnumerable并返回一个新的List
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<dynamic> objs, Func<object, T> action)
        {
            return objs.Select(o => action(o)).Cast<T>();
        }

        /// <summary>
        /// 遍历List并返回一个新的List
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IList<dynamic> objs, Func<object, T> action)
        {
            return objs.Select(o => action(o)).Cast<T>();
        }


        /// <summary>
        /// 遍历数组并返回一个新的List
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this T[] objs, Func<T, T> action)
        {
            return objs.Select(action);
        }

        /// <summary>
        /// 遍历IEnumerable并返回一个新的List
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> objs, Func<T, T> action)
        {
            return objs.Select(action);
        }

        /// <summary>
        /// 遍历List并返回一个新的List
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IList<T> objs, Func<T, T> action)
        {
            return objs.Select(action);
        }

        #endregion

        #region AsyncForEach

        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        public static async void ForEachAsync(this IEnumerable<object> objs, Action<object> action)
        {
            await Task.Run(() => { Parallel.ForEach(objs, action); });
        }

        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        public static async void ForEachAsync<T>(this T[] objs, Action<T> action)
        {
            await Task.Run(() => { Parallel.ForEach(objs, action); });
        }

        /// <summary>
        /// 遍历IEnumerable
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        public static async void ForEachAsync<T>(this IEnumerable<T> objs, Action<T> action)
        {
            await Task.Run(() => { Parallel.ForEach(objs, action); });
        }

        /// <summary>
        /// 遍历List
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        public static async void ForEachAsync<T>(this IList<T> objs, Action<T> action)
        {
            await Task.Run(() => { Parallel.ForEach(objs, action); });
        }

        #endregion

        #region Map

        /// <summary>
        /// 映射到目标类型(浅克隆)
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型</returns>
        public static TDestination MapTo<TDestination>(this object source) where TDestination : new()
        {
            var dest = new TDestination();
            dest.GetType().GetProperties().ForEach(p =>
            {
                p.SetValue(dest, source.GetType().GetProperty(p.Name)?.GetValue(source));
            });
            return dest;
        }

        /// <summary>
        /// 映射到目标类型(浅克隆)
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型</returns>
        public static async Task<TDestination> MapToAsync<TDestination>(this object source) where TDestination : new()
        {
            return await Task.Run(() =>
            {
                var dest = new TDestination();
                dest.GetType().GetProperties().ForEach(p =>
                {
                    p.SetValue(dest, source.GetType().GetProperty(p.Name)?.GetValue(source));
                });
                return dest;
            });
        }

        /// <summary>
        /// 映射到目标类型(深克隆)
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型</returns>
        public static TDestination Map<TDestination>(this object source) where TDestination : new() =>
            JsonConvert.DeserializeObject<TDestination>(JsonConvert.SerializeObject(source));

        /// <summary>
        /// 映射到目标类型(深克隆)
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型</returns>
        public static async Task<TDestination> MapAsync<TDestination>(this object source) where TDestination : new() =>
            await Task.Run(() => JsonConvert.DeserializeObject<TDestination>(JsonConvert.SerializeObject(source)));

        /// <summary>
        /// 复制一个新的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Copy<T>(this T source) where T : new()
        {
            var dest = new T();
            dest.GetType().GetProperties().ForEach(p =>
            {
                p.SetValue(dest, source.GetType().GetProperty(p.Name)?.GetValue(source));
            });
            return dest;
        }

        /// <summary>
        /// 复制到一个现有对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源对象</param>
        /// <param name="dest">目标对象</param>
        /// <returns></returns>
        public static T CopyTo<T>(this T source, T dest) where T : new()
        {
            dest.GetType().GetProperties().ForEach(p =>
            {
                p.SetValue(dest, source.GetType().GetProperty(p.Name)?.GetValue(source));
            });
            return dest;
        }

        /// <summary>
        /// 复制一个新的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static async Task<T> CopyAsync<T>(this T source) where T : new() => await Task.Run(() =>
        {
            var dest = new T();
            dest.GetType().GetProperties().ForEach(p =>
            {
                p.SetValue(dest, source.GetType().GetProperty(p.Name)?.GetValue(source));
            });
            return dest;
        });

        /// <summary>
        /// 映射到目标类型的集合
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型集合</returns>
        public static IEnumerable<TDestination> ToList<TDestination>(this object[] source) where TDestination : new()
        {
            foreach (var o in source)
            {
                var dest = new TDestination();
                dest.GetType().GetProperties().ForEach(p =>
                {
                    p.SetValue(dest, source.GetType().GetProperty(p.Name)?.GetValue(o));
                });
                yield return dest;
            }
        }

        /// <summary>
        /// 映射到目标类型的集合
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型集合</returns>
        public static async Task<IEnumerable<TDestination>> ToListAsync<TDestination>(this object[] source)
            where TDestination : new()
        {
            return await Task.Run(() =>
            {
                IList<TDestination> list = new List<TDestination>();
                foreach (var o in source)
                {
                    var dest = new TDestination();
                    dest.GetType().GetProperties().ForEach(p =>
                    {
                        p.SetValue(dest, source.GetType().GetProperty(p.Name)?.GetValue(o));
                    });
                    list.Add(dest);
                }

                return list;
            });
        }

        /// <summary>
        /// 映射到目标类型的集合
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型集合</returns>
        public static IEnumerable<TDestination> ToList<TDestination>(this IList<dynamic> source)
            where TDestination : new()
        {
            foreach (var o in source)
            {
                var dest = new TDestination();
                dest.GetType().GetProperties().ForEach(p =>
                {
                    p.SetValue(dest, source.GetType().GetProperty(p.Name)?.GetValue(o));
                });
                yield return dest;
            }
        }

        /// <summary>
        /// 映射到目标类型的集合
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型集合</returns>
        public static async Task<IEnumerable<TDestination>> ToListAsync<TDestination>(this IList<dynamic> source)
            where TDestination : new()
        {
            return await Task.Run(() =>
            {
                IList<TDestination> list = new List<TDestination>();
                foreach (var o in source)
                {
                    var dest = new TDestination();
                    dest.GetType().GetProperties().ForEach(p =>
                    {
                        p.SetValue(dest, source.GetType().GetProperty(p.Name)?.GetValue(o));
                    });
                    list.Add(dest);
                }

                return list;
            });
        }

        /// <summary>
        /// 映射到目标类型的集合
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型集合</returns>
        public static IEnumerable<TDestination> ToList<TDestination>(this IEnumerable<dynamic> source)
            where TDestination : new()
        {
            foreach (var o in source)
            {
                var dest = new TDestination();
                dest.GetType().GetProperties().ForEach(p =>
                {
                    p.SetValue(dest, source.GetType().GetProperty(p.Name)?.GetValue(o));
                });
                yield return dest;
            }
        }

        /// <summary>
        /// 映射到目标类型的集合
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型集合</returns>
        public static async Task<IEnumerable<TDestination>> ToListAsync<TDestination>(this IEnumerable<dynamic> source)
            where TDestination : new()
        {
            return await Task.Run(() =>
            {
                IList<TDestination> list = new List<TDestination>();
                foreach (var o in source)
                {
                    var dest = new TDestination();
                    dest.GetType().GetProperties().ForEach(p =>
                    {
                        p.SetValue(dest, source.GetType().GetProperty(p.Name)?.GetValue(o));
                    });
                    list.Add(dest);
                }

                return list;
            });
        }

        #endregion

        /// <summary>
        /// 转换成json字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToJsonString(this object source) => JsonConvert.SerializeObject(source,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

        #region 数字互转

        /// <summary>
        /// 字符串转int
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>int类型的数字</returns>
        public static int ToInt32(this string s)
        {
            int.TryParse(s, out var result);
            return result;
        }

        /// <summary>
        /// 字符串转long
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>int类型的数字</returns>
        public static long ToInt64(this string s)
        {
            long.TryParse(s, out var result);
            return result;
        }

        /// <summary>
        /// 字符串转double
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>double类型的数据</returns>
        public static double ToDouble(this string s)
        {
            double.TryParse(s, out var result);
            return result;
        }

        /// <summary>
        /// 字符串转decimal
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>int类型的数字</returns>
        public static decimal ToDecimal(this string s)
        {
            decimal.TryParse(s, out var result);
            return result;
        }

        /// <summary>
        /// 字符串转decimal
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>int类型的数字</returns>
        public static decimal ToDecimal(this double s)
        {
            return new decimal(s);
        }

        /// <summary>
        /// 字符串转double
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>double类型的数据</returns>
        public static double ToDouble(this decimal s)
        {
            return (double)s;
        }

        /// <summary>
        /// 将double转换成int
        /// </summary>
        /// <param name="num">double类型</param>
        /// <returns>int类型</returns>
        public static int ToInt32(this double num)
        {
            return (int)Math.Floor(num);
        }

        /// <summary>
        /// 将double转换成int
        /// </summary>
        /// <param name="num">double类型</param>
        /// <returns>int类型</returns>
        public static int ToInt32(this decimal num)
        {
            return (int)Math.Floor(num);
        }

        /// <summary>
        /// 字符串转long类型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultResult">转换失败的默认值</param>
        /// <returns></returns>
        public static long ToLong(this string str, long defaultResult)
        {
            if (!long.TryParse(str, out var result))
            {
                result = defaultResult;
            }

            return result;
        }

        /// <summary>
        /// 将int转换成double
        /// </summary>
        /// <param name="num">int类型</param>
        /// <returns>int类型</returns>
        public static double ToDouble(this int num)
        {
            return num * 1.0;
        }

        /// <summary>
        /// 将int转换成decimal
        /// </summary>
        /// <param name="num">int类型</param>
        /// <returns>int类型</returns>
        public static decimal ToDecimal(this int num)
        {
            return (decimal)(num * 1.0);
        }

        #endregion

        #region 检测字符串中是否包含列表中的关键词

        /// <summary>
        /// 检测字符串中是否包含列表中的关键词
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="keys">关键词列表</param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns></returns>
        public static bool Contains(this string s, IEnumerable<string> keys, bool ignoreCase = true)
        {
            if (ignoreCase)
            {
                return Regex.IsMatch(s.ToLower(), string.Join("|", keys).ToLower());
            }

            return Regex.IsMatch(s, string.Join("|", keys));
        }

        #endregion

        #region 匹配Email

        /// <summary>
        /// 匹配Email
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="isMatch">是否匹配成功，若返回true，则会得到一个Match对象，否则为null</param>
        /// <returns>匹配对象</returns>
        public static Match MatchEmail(this string s, out bool isMatch)
        {
            var match = Regex.Match(s, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            isMatch = match.Success;
            return isMatch ? match : null;
        }

        /// <summary>
        /// 匹配Email
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>是否匹配成功</returns>
        public static bool MatchEmail(this string s)
        {
            MatchEmail(s, out var success);
            return success;
        }

        #endregion

        #region 匹配完整的URL

        /// <summary>
        /// 匹配完整格式的URL
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="isMatch">是否匹配成功，若返回true，则会得到一个Match对象，否则为null</param>
        /// <returns>匹配对象</returns>
        public static Uri MatchUrl(this string s, out bool isMatch)
        {
            try
            {
                isMatch = true;
                return new Uri(s);
            }
            catch
            {
                isMatch = false;
                return null;
            }
        }

        /// <summary>
        /// 匹配完整格式的URL
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>是否匹配成功</returns>
        public static bool MatchUrl(this string s)
        {
            MatchUrl(s, out var isMatch);
            return isMatch;
        }

        #endregion

        #region 权威校验身份证号码

        /// <summary>
        /// 根据GB11643-1999标准权威校验中国身份证号码的合法性
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>是否匹配成功</returns>
        public static bool MatchIdentifyCard(this string s)
        {
            const string address =
                "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            switch (s.Length)
            {
                case 18 when long.TryParse(s.Remove(17), out var n) == false || n < Math.Pow(10, 16) ||
                             long.TryParse(s.Replace('x', '0').Replace('X', '0'), out n) == false:
                //省份验证  
                case 18 when address.IndexOf(s.Remove(2), StringComparison.Ordinal) == -1:
                    return false; //数字验证  
                case 18:
                    {
                        var birth = s.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                        if (!DateTime.TryParse(birth, out _))
                        {
                            return false; //生日验证  
                        }

                        var arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
                        var wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
                        var ai = s.Remove(17).ToCharArray();
                        var sum = 0;
                        for (var i = 0; i < 17; i++)
                        {
                            sum += wi[i].ToInt32() * ai[i].ToString().ToInt32();
                        }

                        Math.DivRem(sum, 11, out var y);
                        return arrVarifyCode[y] == s.Substring(17, 1).ToLower();
                    }
                case 15 when long.TryParse(s, out var n) == false || n < Math.Pow(10, 14):
                //省份验证  
                case 15 when address.IndexOf(s.Remove(2), StringComparison.Ordinal) == -1:
                    return false; //数字验证  
                case 15:
                    {
                        var birth = s.Substring(6, 6).Insert(4, "-").Insert(2, "-");
                        return DateTime.TryParse(birth, out _);
                    }
                default:
                    return false;
            }
        }

        #endregion

        #region 校验IP地址的合法性

        /// <summary>
        /// 校验IP地址的正确性，同时支持IPv4和IPv6
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="isMatch">是否匹配成功，若返回true，则会得到一个Match对象，否则为null</param>
        /// <returns>匹配对象</returns>
        public static IPAddress MatchInetAddress(this string s, out bool isMatch)
        {
            isMatch = IPAddress.TryParse(s, out var ip);
            return ip;
        }

        /// <summary>
        /// 校验IP地址的正确性，同时支持IPv4和IPv6
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>是否匹配成功</returns>
        public static bool MatchInetAddress(this string s)
        {
            MatchInetAddress(s, out var success);
            return success;
        }

        #endregion

        #region 校验手机号码的正确性

        /// <summary>
        /// 匹配手机号码
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="isMatch">是否匹配成功，若返回true，则会得到一个Match对象，否则为null</param>
        /// <returns>匹配对象</returns>
        public static Match MatchPhoneNumber(this string s, out bool isMatch)
        {
            var match = Regex.Match(s, @"^((1[3,5,6,8][0-9])|(14[5,7])|(17[0,1,3,6,7,8])|(19[8,9]))\d{8}$");
            isMatch = match.Success;
            return isMatch ? match : null;
        }

        /// <summary>
        /// 匹配手机号码
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>是否匹配成功</returns>
        public static bool MatchPhoneNumber(this string s)
        {
            MatchPhoneNumber(s, out var success);
            return success;
        }

        #endregion

        /// <summary>
        /// 严格比较两个对象是否是同一对象
        /// </summary>
        /// <param name="this">自己</param>
        /// <param name="o">需要比较的对象</param>
        /// <returns>是否同一对象</returns>
        public new static bool ReferenceEquals(this object @this, object o) => object.ReferenceEquals(@this, o);

        /// <summary>
        /// 判断字符串是否为空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

        /// <summary>
        /// 类型直转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T To<T>(this IConvertible value)
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 字符串转时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string value)
        {
            DateTime.TryParse(value, out var result);
            return result;
        }

        /// <summary>
        /// 字符串转Guid
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string s)
        {
            return Guid.Parse(s);
        }

        /// <summary>
        /// 根据正则替换
        /// </summary>
        /// <param name="input"></param>
        /// <param name="regex">正则表达式</param>
        /// <param name="replacement">新内容</param>
        /// <returns></returns>
        public static string Replace(this string input, Regex regex, string replacement)
        {
            return regex.Replace(input, replacement);
        }

        /// <summary>
        /// 生成唯一短字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="chars">可用字符数数量，0-9,a-z,A-Z</param>
        /// <returns></returns>
        public static string CreateShortToken(this string str, byte chars = 36)
        {
            var nf = new NumberFormater(chars);
            return nf.ToString((DateTime.Now.Ticks - 630822816000000000) * 100 + Stopwatch.GetTimestamp() % 100);
        }

        /// <summary>
        /// 十进制转任意进制
        /// </summary>
        /// <param name="num"></param>
        /// <param name="bin">进制</param>
        /// <returns></returns>
        public static string ToBinary(this long num, int bin)
        {
            var nf = new NumberFormater(bin);
            return nf.ToString(num);
        }

        /// <summary>
        /// 十进制转任意进制
        /// </summary>
        /// <param name="num"></param>
        /// <param name="bin">进制</param>
        /// <returns></returns>
        public static string ToBinary(this int num, int bin)
        {
            var nf = new NumberFormater(bin);
            return nf.ToString(num);
        }

        /// <summary>
        /// 按字段去重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var hash = new HashSet<TKey>();
            return source.Where(p => hash.Add(keySelector(p)));
        }

        /// <summary>
        /// 将小数截断为8位
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double Digits8(this double num)
        {
            return (long)(num * 1E+8) * 1e-8;
        }

        /// <summary>
        /// 判断IP地址在不在某个IP地址段
        /// </summary>
        /// <param name="input">需要判断的IP地址</param>
        /// <param name="begin">起始地址</param>
        /// <param name="ends">结束地址</param>
        /// <returns></returns>
        public static bool IpAddressInRange(this string input, string begin, string ends)
        {
            var current = IpToId(input);
            return current >= IpToId(begin) && current <= IpToId(ends);
        }

        /// <summary>
        /// IP地址转换成数字
        /// </summary>
        /// <param name="addr">IP地址</param>
        /// <returns>数字,输入无效IP地址返回0</returns>
        private static uint IpToId(string addr)
        {
            if (!IPAddress.TryParse(addr, out var ip))
            {
                return 0;
            }

            var bInt = ip.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bInt);
            }

            return BitConverter.ToUInt32(bInt, 0);
        }

        /// <summary>
        /// 判断IP是否是私有地址
        /// </summary>
        /// <param name="myIpAddress"></param>
        /// <returns></returns>
        public static bool IsPrivateIp(this IPAddress myIpAddress)
        {
            if (IPAddress.IsLoopback(myIpAddress)) return true;

            if (myIpAddress.AddressFamily != AddressFamily.InterNetwork)
                return false;

            var ipBytes = myIpAddress.GetAddressBytes();
            // 10.0.0.0/24 
            if (ipBytes[0] == 10)
            {
                return true;
            }
            // 169.254.0.0/16
            if (ipBytes[0] == 169 && ipBytes[1] == 254)
            {
                return true;
            }
            // 172.16.0.0/16
            if (ipBytes[0] == 172 && ipBytes[1] == 16)
            {
                return true;
            }
            // 192.168.0.0/16
            return ipBytes[0] == 192 && ipBytes[1] == 168;
        }

        /// <summary>
        /// 判断IP是否是私有地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsPrivateIp(this string ip)
        {
            if (MatchInetAddress(ip))
            {
                return IsPrivateIp(IPAddress.Parse(ip));
            }
            throw new ArgumentException(ip + "不是一个合法的ip地址");
        }

        /// <summary>
        /// 判断url是否是外部地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsExternalAddress(this string url)
        {
            var uri = new Uri(url);
            switch (uri.HostNameType)
            {
                case UriHostNameType.Dns:
                    var ipHostEntry = Dns.GetHostEntry(uri.DnsSafeHost);
                    if (ipHostEntry.AddressList.Where(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork).Any(ipAddress => !ipAddress.IsPrivateIp()))
                    {
                        return true;
                    }
                    break;
                case UriHostNameType.IPv4:
                    return !IPAddress.Parse(uri.DnsSafeHost).IsPrivateIp();
            }
            return false;
        }

        /// <summary>
        /// 生成真正的随机数
        /// </summary>
        /// <param name="r"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static int StrictNext(this Random r, int mix, int max = int.MaxValue)
        {
            return RandomManager.CreateRandomNumber(mix, max);
        }

        public static double StrictNext(this Random r, double mix, double max = int.MaxValue)
        {
            return RandomManager.CreateRandomNumber(mix, max);
        }

        /// <summary>
        /// 将流转换为内存流
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="dispose">保存后是否释放源流</param>
        /// <returns></returns>
        public static MemoryStream SaveAsMemoryStream(this Stream stream, bool dispose = false)
        {
            return new MemoryStream(stream.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ToArray(this Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary>
        /// 添加多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="values"></param>
        public static void AddRange<T>(this ICollection<T> @this, params T[] values)
        {
            foreach (var obj in values)
            {
                @this.Add(obj);
            }
        }

        /// <summary>
        /// 添加符合条件的多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="predicate"></param>
        /// <param name="values"></param>
        public static void AddRangeIf<T>(this ICollection<T> @this, Func<T, bool> predicate, params T[] values)
        {
            foreach (var obj in values)
            {
                if (predicate(obj))
                {
                    @this.Add(obj);
                }
            }
        }

        /// <summary>
        /// 添加不重复的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="values"></param>
        public static void AddRangeIfNotContains<T>(this ICollection<T> @this, params T[] values)
        {
            foreach (var obj in values)
            {
                if (!@this.Contains(obj))
                {
                    @this.Add(obj);
                }
            }
        }

        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this short value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this int value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this long value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string @this)
        {
            return Activator.CreateInstance<ASCIIEncoding>().GetBytes(@this);
        }

        /// <summary>
        /// 添加或更新键值对
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="this"></param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(new KeyValuePair<TKey, TValue>(key, value));
            }
            else
            {
                @this[key] = value;
            }

            return @this[key];
        }

        /// <summary>
        /// 添加或更新键值对
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="this"></param>
        /// <param name="key">键</param>
        /// <param name="addValue">添加时的值</param>
        /// <param name="updateValueFactory">更新时的操作</param>
        /// <returns></returns>
        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(new KeyValuePair<TKey, TValue>(key, addValue));
            }
            else
            {
                @this[key] = updateValueFactory(key, @this[key]);
            }

            return @this[key];
        }

        /// <summary>
        /// 添加或更新键值对
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="this"></param>
        /// <param name="key">键</param>
        /// <param name="addValueFactory">添加时的操作</param>
        /// <param name="updateValueFactory">更新时的操作</param>
        /// <returns></returns>
        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(new KeyValuePair<TKey, TValue>(key, addValueFactory(key)));
            }
            else
            {
                @this[key] = updateValueFactory(key, @this[key]);
            }

            return @this[key];
        }

        /// <summary>
        /// 移除符合条件的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="where"></param>
        public static void RemoveWhere<T>(this ICollection<T> @this, Func<T, bool> @where)
        {
            foreach (var obj in @this.Where(@where).ToList())
            {
                @this.Remove(obj);
            }
        }

        #region 小数转时间

        /// <summary>
        /// 将秒转换成毫秒。
        /// </summary>
        /// <param name="second">秒数。</param>
        /// <returns>毫秒数。</returns>
        public static double GetSecondToMilliseconds(this double second)
        {
            return second * 1000;
        }

        /// <summary>
        /// 将分转换成毫秒。
        /// </summary>
        /// <param name="minute">分。</param>
        /// <returns>毫秒数。</returns>
        public static double GetMinuteToMilliseconds(this double minute)
        {
            return GetSecondToMilliseconds(60) * minute;
        }

        /// <summary>
        /// 将小时转换成毫秒。
        /// </summary>
        /// <param name="hour">小时。</param>
        /// <returns>毫秒数。</returns>
        public static double GetHourToMilliseconds(this double hour)
        {
            return GetMinuteToMilliseconds(60) * hour;
        }

        /// <summary>
        /// 将天转换成毫秒。
        /// </summary>
        /// <param name="day">天数。</param>
        /// <returns>毫秒数。</returns>
        public static double GetDayToMilliseconds(this double day)
        {
            return GetHourToMilliseconds(24) * day;
        }

        #endregion

        /// <summary>
        /// 将指定的时间与当前时间的间隔数转换成毫秒。
        /// </summary>
        /// <param name="executeTime">指定的时间。</param>
        /// <returns>毫秒数。</returns>
        public static double GetDateToMilliseconds(this DateTime executeTime)
        {
            if (executeTime <= DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(executeTime), "执行时间不可以是过去的时间");

            var span = executeTime - DateTime.Now;

            return span.TotalMilliseconds;
        }

        /// <summary>
        /// 获得默认时间。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetDefaultTime(this DateTime time) => new DateTime(1970, 1, 1);

        /// <summary>
        /// 获得当前时间的时间戳（秒级）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetTimeStamp(this DateTime time)
        {
#pragma warning disable 618
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
#pragma warning restore 618

            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// 获得当前时间的时间戳（毫秒级）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long GetTimeStampLong(this DateTime time)
        {
#pragma warning disable 618
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
#pragma warning restore 618

            return (long)(time - startTime).TotalMilliseconds;
        }

        /// <summary>
        /// 获得去除时分秒后的当前时间。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetToday(this DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day);
        }

        /// <summary>
        /// 获得指定时间当天的开始时间和结束时间。
        /// </summary>
        /// <param name="time">当前时间</param>
        /// <returns>开始时间和结束时间</returns>
        public static DateTime[] GetTodayScope(this DateTime time)
        {
            var startTime = new DateTime(time.Year, time.Month, time.Day);
            var endTime = startTime.AddDays(1).AddSeconds(-1);

            return new[]
            {
                startTime,
                endTime
            };
        }

        /// <summary>
        /// 获得字符串中文件格式的扩展名。
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static string GetExtension(this string sources)
        {
            var i = sources.LastIndexOf('.');
            if (i < 0 || i + 1 >= sources.Length - 1)
                return "";
            return sources.Substring(i + 1);
        }

        public static bool IsNullOrWhiteSpace(this string sources)
        {
            return string.IsNullOrWhiteSpace(sources);
        }
    }
}