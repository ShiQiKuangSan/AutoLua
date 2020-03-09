using System;
using Android.Content;
using Android.OS;
using Android.Net;
using Android.Telephony;
using Android.Net.Wifi;
using Android.Provider;
using Java.Interop;

namespace AutoLua.Droid.Utils
{
    public static class NetworkUtils
    {
        public const int NetworkWifi = 1; // wifi network
        public const int Network4G = 4; // "4G" networks
        public const int Network3G = 3; // "3G" networks
        public const int Network2G = 2; // "2G" networks
        public const int NetworkUnknown = 5; // unknown network
        public const int NetworkNo = -1; // no network

        private const int NetworkTypeGsm = 16;
        private const int NetworkTypeTdScdma = 17;
        private const int NetworkTypeIwlan = 18;

        /// <summary>
        /// 打开网络设置界面
        /// 3.0以下打开设置界面
        /// </summary>
        /// <param name="context">上下文</param>
        public static void OpenWirelessSettings(Context context)
        {
            if (Build.VERSION.SdkInt > BuildVersionCodes.GingerbreadMr1) //  10
            {
                context.StartActivity(new Intent(Settings.ActionSettings));
            }
            else
            {
                context.StartActivity(new Intent(Settings.ActionWirelessSettings));
            }
        }

        /// <summary>
        /// 获取活动网络信息
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        private static NetworkInfo GetActiveNetworkInfo(Context context)
        {
            var cm =  context.GetSystemService(Context.ConnectivityService).JavaCast<ConnectivityManager>();
            return cm.ActiveNetworkInfo;
        }


        /// <summary>
        /// 判断网络是否可用
        /// <p>需添加权限 {@code <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>}</p>
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>{@code true}: 可用 {@code false}: 不可用</returns>
        public static bool IsAvailable(Context context)
        {
            var info = GetActiveNetworkInfo(context);
            return info != null && info.IsAvailable;
        }

        /// <summary>
        /// 判断网络是否连接
        /// <p>需添加权限 {@code <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>}</p>
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public static bool IsConnected(Context context)
        {
            var info = GetActiveNetworkInfo(context);
            return info != null && info.IsConnected;
        }


        /// <summary>
        /// 判断网络是否是4G
        /// <p>需添加权限 {@code <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>}</p>
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public static bool Is4G(Context context)
        {
            var info = GetActiveNetworkInfo(context);
            var tm = TelephonyManager.FromContext(context);
            return info != null && info.IsAvailable && tm.NetworkType == NetworkType.Lte;
        }


        /// <summary>
        /// 判断wifi是否连接状态
        /// <p>需添加权限 {@code <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>}</p>
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public static bool IsWifiConnected(Context context)
        {
            var cm = context.GetSystemService(Context.ConnectivityService).JavaCast<ConnectivityManager>();
            
            return cm?.ActiveNetworkInfo != null && cm.ActiveNetworkInfo.Type == ConnectivityType.Wifi;
        }


        /// <summary>
        /// 获取移动网络运营商名称
        /// <p>如中国联通、中国移动、中国电信</p>
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>移动网络运营商名称</returns>
        public static string GetNetworkOperatorName(Context context)
        {
            var tm = context.GetSystemService(Context.TelephonyService).JavaCast<TelephonyManager>();
            return tm?.NetworkOperatorName;
        }


        /// <summary>
        /// 获取移动终端类型
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>
        /// 手机制式:
        ///     0 手机制式未知
        ///     1 手机制式为GSM，移动和联通
        ///     2 手机制式为CDMA，电信
        /// </returns>
        public static int GetPhoneType(Context context)
        {
            var tm =  context.GetSystemService(Context.TelephonyService).JavaCast<TelephonyManager>();
            return tm != null ? (int) tm.PhoneType : -1;
        }



        /// <summary>
        /// 获取当前的网络类型(WIFI,2G,3G,4G)
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>
        /// 网络类型:
        ///     <li>{@link TelephonyManager#PHONE_TYPE_NONE } : 0 手机制式未知</li>
        ///     <li>{@link TelephonyManager#PHONE_TYPE_GSM  } : 1 手机制式为GSM，移动和联通</li>
        ///     <li>{@link TelephonyManager#PHONE_TYPE_CDMA } : 2 手机制式为CDMA，电信</li>
        ///     <li>{@link TelephonyManager#PHONE_TYPE_SIP  } : 3</li>
        /// </returns>
        public static int GetNetWorkType(Context context)
        {
            var netType = NetworkNo;
            var info = GetActiveNetworkInfo(context);
            
            if (info == null || !info.IsAvailable)
                return netType;
            
            switch (info.Type)
            {
                case ConnectivityType.Wifi:
                    netType = NetworkWifi;
                    break;
                case ConnectivityType.Mobile:
                {
                    var tm = TelephonyManager.FromContext(context);
                    switch (tm.NetworkType)
                    {
                        case NetworkType.Gsm:
                        case NetworkType.Gprs:
                        case NetworkType.Cdma:
                        case NetworkType.Edge:
                        case NetworkType.OneXrtt:
                        case NetworkType.Iden:
                            netType = Network2G;
                            break;

                        case NetworkType.TdScdma:
                        case NetworkType.EvdoA:
                        case NetworkType.Umts:
                        case NetworkType.Evdo0:
                        case NetworkType.Hsdpa:
                        case NetworkType.Hsupa:
                        case NetworkType.Hspa:
                        case NetworkType.EvdoB:
                        case NetworkType.Ehrpd:
                        case NetworkType.Hspap:
                            netType = Network3G;
                            break;

                        case NetworkType.Iwlan:
                        case NetworkType.Lte:
                            netType = Network4G;
                            break;
                        default:

                            var subtypeName = info.SubtypeName;
                            if (subtypeName.Equals("TD-SCDMA", StringComparison.InvariantCultureIgnoreCase)
                                || subtypeName.Equals("WCDMA", StringComparison.InvariantCultureIgnoreCase)
                                || subtypeName.Equals("CDMA2000", StringComparison.InvariantCultureIgnoreCase))
                            {
                                netType = Network3G;
                            }
                            else
                            {
                                netType = NetworkUnknown;
                            }
                            break;
                    }

                    break;
                }
                default:
                    netType = NetworkUnknown;
                    break;
            }

            return netType;
        }


        /// <summary>
        /// 获取当前的网络类型(WIFI,2G,3G,4G)
        /// <p>依赖上面的方法</p>
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>
        /// 网络类型名称:
        ///     <li>NETWORK_WIFI   </li>
        ///     <li>NETWORK_4G     </li>
        ///     <li>NETWORK_3G     </li>
        ///     <li>NETWORK_2G     </li>
        ///     <li>NETWORK_UNKNOWN</li>
        ///     <li>NETWORK_NO     </li>
        /// </returns>
        public static string GetNetWorkTypeName(Context context)
        {
            switch (GetNetWorkType(context))
            {
                case NetworkWifi:
                    return "NETWORK_WIFI";
                case Network4G:
                    return "NETWORK_4G";
                case Network3G:
                    return "NETWORK_3G";
                case Network2G:
                    return "NETWORK_2G";
                case NetworkNo:
                    return "NETWORK_NO";
                default:
                    return "NETWORK_UNKNOWN";
            }
        }


        /// <summary>
        /// 获取当前连接wifi的名称
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetConnectWifiSsid(Context context)
        {
            if (IsWifiConnected(context))
            {
                var wifiManager = context.GetSystemService(Context.WifiService).JavaCast<WifiManager>();
                var wifiInfo = wifiManager.ConnectionInfo;
                return wifiInfo.SSID;
            }

            return null;
        }


        /// <summary>
        /// 获取当前连接wifi的名称
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetConnectWifiIp(Context context)
        {
            if (!IsWifiConnected(context)) 
                return null;
            
            var wifiManager = context.GetSystemService(Context.WifiService).JavaCast<WifiManager>();
            var wifiInfo = wifiManager.ConnectionInfo;
            var ipAddress = wifiInfo.IpAddress;
            if (ipAddress == 0)
            {
                return null;
            }

            return (ipAddress & 0xff) + "." + (ipAddress >> 8 & 0xff) + "."
                   + (ipAddress >> 16 & 0xff) + "." + (ipAddress >> 24 & 0xff);
        }
    }
}