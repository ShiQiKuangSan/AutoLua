using System;
using System.Linq;
using System.Net;

namespace AutoLua.Core.LuaScript.ApiCommon.Http.Models
{
    /// <summary>
    /// Http返回参数类
    /// </summary>
    public class HttpResult
    {
        /// <summary>
        /// Http请求返回的Cookie
        /// </summary>
        public string Cookie { get; set; } = string.Empty;

        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection { get; set; }

        /// <summary>
        /// 返回的String类型数据 只有ResultType.String时才返回数据，其它情况为空
        /// </summary>
        public string Html { get; set; } = string.Empty;

        /// <summary>
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空
        /// </summary>
        public byte[] ResultByte { get; set; }

        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection Header { get; set; }

        /// <summary>
        /// 返回状态说明
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// 返回状态码,默认为OK
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 最后访问的URl
        /// </summary>
        public string ResponseUri { get; set; }

        /// <summary>
        /// 获取重定向的URl
        /// </summary>
        public string RedirectUrl
        {
            get
            {
                try
                {
                    if (Header != null && Header.Count > 0)
                    {
                        if (Header.AllKeys.Any(k => k.ToLower().Contains("location")))
                        {
                            var baseurl = Header["location"].ToString().Trim();
                            var locationurl = baseurl.ToLower();

                            if (string.IsNullOrWhiteSpace(locationurl))
                                return baseurl;

                            var b = locationurl.StartsWith("http://") || locationurl.StartsWith("https://");
                            if (!b)
                            {
                                baseurl = new Uri(new Uri(ResponseUri), baseurl).AbsoluteUri;
                            }

                            return baseurl;
                        }
                    }
                }
                catch
                {
                    // ignored
                }

                return string.Empty;
            }
        }
    }
}