﻿using AutoLua.Core.Common;

namespace AutoLua.Droid.HttpServers
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class AccessibilityHttpServer : HttpServer.HttpService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">IP地址</param>
        public AccessibilityHttpServer(int port)
            : base("127.0.0.1", port)
        {

        }
    }
}