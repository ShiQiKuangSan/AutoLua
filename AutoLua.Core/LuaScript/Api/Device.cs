using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using AutoLua.Core.Common;
using Java.Lang;
using Java.Net;
using Java.Util;
using Xamarin.Forms;
using Settings = Android.Provider.Settings;
using Stream = Android.Media.Stream;

namespace AutoLua.Core.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class Device
    {
        /// <summary>
        /// 设备屏幕分辨率宽度。例如1080。
        /// </summary>
        public int width => ScreenMetrics.Instance.DeviceScreenWidth;

        /// <summary>
        /// 设备屏幕分辨率高度。例如1920。
        /// </summary>
        public int height => ScreenMetrics.Instance.DeviceScreenHeight;

        /// <summary>
        /// 修订版本号，或者诸如"M4-rc20"的标识。
        /// </summary>
        public string buildId = Build.Id;

        public string buildDisplay = Build.Display;

        /// <summary>
        /// 整个产品的名称。
        /// </summary>
        public string product = Build.Product;

        /// <summary>
        /// 设备的主板(?)型号。
        /// </summary>
        public string board = Build.Board;

        /// <summary>
        /// 与产品或硬件相关的厂商品牌，如"Xiaomi", "Huawei"等。
        /// </summary>
        public string brand = Build.Brand;

        /// <summary>
        /// 设备在工业设计中的名称。
        /// </summary>
        public string device = Build.Device;

        /// <summary>
        /// 设备型号。
        /// </summary>
        public string model = Build.Model;

        /// <summary>
        /// 设备Bootloader的版本。
        /// </summary>
        public string bootloader = Build.Bootloader;

        /// <summary>
        /// 设备的硬件名称(来自内核命令行或者/proc)。
        /// </summary>
        public string hardware = Build.Hardware;

        /// <summary>
        /// 构建(build)的唯一标识码。
        /// </summary>
        public string fingerprint = Build.Fingerprint;

        /// <summary>
        /// 安卓系统API版本。例如安卓4.4的sdkInt为19。
        /// </summary>
        public BuildVersionCodes sdkInt = Build.VERSION.SdkInt;

        /// <summary>
        /// 基础源控件用来表示此内部版本的内部值。 例如，一个perforce变更列表编号或git哈希。
        /// </summary>
        public string incremental = Build.VERSION.Incremental;

        /// <summary>
        /// Android系统版本号。例如"5.0", "7.1.1"。
        /// </summary>
        public string release = Build.VERSION.Release;

        /// <summary>
        /// 产品所基于的基本操作系统。
        /// </summary>
        public string baseOS;

        /// <summary>
        /// 安全补丁程序级别。
        /// </summary>
        public string securityPatch;

        public Device()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                baseOS = Build.VERSION.BaseOs;
                securityPatch = Build.VERSION.SecurityPatch;
            }
            else
            {
                baseOS = string.Empty;
                securityPatch = string.Empty;
            }
        }

        /// <summary>
        /// 开发代号，例如发行版是"REL"。
        /// </summary>
        public string codename = Build.VERSION.Codename;

        /// <summary>
        /// 硬件序列号。
        /// </summary>
        public string serial = Build.Serial;

        private PowerManager.WakeLock _mWakeLock;

        /// <summary>
        /// 设备的IMEI.
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public string IMEI
        {
            get
            {
                CheckReadPhoneStatePermission();
                try
                {
                    return GetSystemService<TelephonyManager>(Context.TelephonyService).DeviceId;
                }
                catch (System.Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Android ID为一个用16进制字符串表示的64位整数，在设备第一次使用时随机生成，之后不会更改，除非恢复出厂设置。
        /// </summary>
        public string AndroidId
        {
            get
            {
                var context = AppUtils.AppContext;
                return Settings.Secure.GetString(context.ContentResolver, Settings.Secure.AndroidId);
            }
        }


        private const string FakeMacAddress = "02:00:00:00:00:00";

        /// <summary>
        /// 返回设备的Mac地址。该函数需要在有WLAN连接的情况下才能获取，否则会返回空。
        /// </summary>
        public string MacAddress
        {
            get
            {
                var wifiMan = GetSystemService<WifiManager>(Context.WifiService);

                if (wifiMan == null)
                {
                    return null;
                }

                var wifiInf = wifiMan.ConnectionInfo;

                if (wifiInf == null)
                {
                    return GetMacByFile();
                }

                var mac = wifiInf.MacAddress;
                if (FakeMacAddress == mac)
                {
                    mac = null;
                }

                if (mac == null)
                {
                    mac = GetMacByInterface() ?? GetMacByFile();
                }
                return mac;
            }
        }

        /// <summary>
        /// 返回当前的(手动)亮度。范围为0~255。
        /// </summary>
        public string Brightness
        {
            get
            {
                var context = AppUtils.AppContext;
                return Settings.Secure.GetString(context.ContentResolver, Settings.System.ScreenBrightness);
            }
        }

        /// <summary>
        /// 返回当前亮度模式，0为手动亮度，1为自动亮度。
        /// </summary>
        public string BrightnessMode
        {
            get
            {
                var context = AppUtils.AppContext;
                return Settings.Secure.GetString(context.ContentResolver, Settings.System.ScreenBrightnessMode);
            }
        }

        /// <summary>
        /// 设置当前手动亮度。如果当前是自动亮度模式，该函数不会影响屏幕的亮度。
        /// 此函数需要"修改系统设置"的权限。如果没有该权限，会抛出SecurityException并跳转到权限设置界面。
        /// </summary>
        /// <param name="value">亮度，范围0~255</param>
        public void SetBrightness(int value)
        {
            CheckWriteSettingsPermission();
            var context = AppUtils.AppContext;
            Settings.System.PutInt(context.ContentResolver, Settings.System.ScreenBrightness, value);
        }

        /// <summary>
        /// 设置当前亮度模式。
        /// 此函数需要"修改系统设置"的权限。如果没有该权限，会抛出SecurityException并跳转到权限设置界面。
        /// </summary>
        /// <param name="value">亮度模式，0为手动亮度，1为自动亮度</param>
        public void SetBrightnessMode(int value)
        {
            CheckWriteSettingsPermission();
            var context = AppUtils.AppContext;
            Settings.System.PutInt(context.ContentResolver, Settings.System.ScreenBrightnessMode, value);
        }

        /// <summary>
        /// 返回当前媒体音量。
        /// </summary>
        public int MusicVolume => GetSystemService<AudioManager>(Context.AudioService).GetStreamVolume(Stream.Music);

        /// <summary>
        /// 返回当前通知音量。
        /// </summary>
        public int NotificationVolume => GetSystemService<AudioManager>(Context.AudioService).GetStreamVolume(Stream.Notification);

        /// <summary>
        /// 返回当前闹钟音量。
        /// </summary>
        public int AlarmVolume => GetSystemService<AudioManager>(Context.AudioService).GetStreamVolume(Stream.Alarm);

        /// <summary>
        /// 返回媒体音量的最大值。
        /// </summary>
        public int MusicMaxVolume => GetSystemService<AudioManager>(Context.AudioService).GetStreamMaxVolume(Stream.Music);

        /// <summary>
        /// 返回通知音量的最大值。
        /// </summary>
        public int NotificationMaxVolume => GetSystemService<AudioManager>(Context.AudioService).GetStreamMaxVolume(Stream.Notification);

        /// <summary>
        /// 返回闹钟音量的最大值。
        /// </summary>
        public int AlarmMaxVolume => GetSystemService<AudioManager>(Context.AudioService).GetStreamMaxVolume(Stream.Alarm);

        /// <summary>
        /// 设置当前媒体音量。
        /// 此函数需要"修改系统设置"的权限。如果没有该权限，会抛出SecurityException并跳转到权限设置界面。
        /// </summary>
        /// <param name="i">音量</param>
        public void SetMusicVolume(int i)
        {
            CheckWriteSettingsPermission();
            GetSystemService<AudioManager>(Context.AudioService).SetStreamVolume(Stream.Music, i, VolumeNotificationFlags.AllowRingerModes);
        }

        /// <summary>
        /// 设置当前闹钟音量。
        /// 此函数需要"修改系统设置"的权限。如果没有该权限，会抛出SecurityException并跳转到权限设置界面。
        /// </summary>
        /// <param name="i">音量</param>
        public void SetAlarmVolume(int i)
        {
            CheckWriteSettingsPermission();
            GetSystemService<AudioManager>(Context.AudioService).SetStreamVolume(Stream.Alarm, i, VolumeNotificationFlags.AllowRingerModes);
        }

        /// <summary>
        /// 设置当前通知音量。
        /// 此函数需要"修改系统设置"的权限。如果没有该权限，会抛出SecurityException并跳转到权限设置界面。
        /// </summary>
        /// <param name="i">音量</param>
        public void SetNotificationVolume(int i)
        {
            CheckWriteSettingsPermission();
            GetSystemService<AudioManager>(Context.AudioService).SetStreamVolume(Stream.Notification, i, VolumeNotificationFlags.AllowRingerModes);
        }

        /// <summary>
        /// 返回当前电量百分比。
        /// 0.0~100.0的浮点数
        /// </summary>
        public float Battery
        {
            get
            {
                var context = AppUtils.AppContext;

                var batteryIntent = context.RegisterReceiver(null, new IntentFilter(Intent.ActionBatteryChanged));
                if (batteryIntent == null)
                    return -1;

                var level = batteryIntent.GetIntExtra(BatteryManager.ExtraLevel, -1);
                var scale = batteryIntent.GetIntExtra(BatteryManager.ExtraScale, -1);
                var battery = ((float)level / scale) * 100.0f;
                return (battery * 10) / 10;
            }
        }

        /// <summary>
        /// 返回设备内存总量，单位字节(B)。1MB = 1024 * 1024B。
        /// </summary>
        public float TotalMem
        {
            get
            {
                var activityManager = GetSystemService<ActivityManager>(Context.ActivityService);
                var info = new ActivityManager.MemoryInfo();
                activityManager.GetMemoryInfo(info);

                return info.TotalMem;
            }
        }

        /// <summary>
        /// 返回设备当前可用的内存，单位字节(B)。
        /// </summary>
        public float AvailMem
        {
            get
            {
                var activityManager = GetSystemService<ActivityManager>(Context.ActivityService);
                var info = new ActivityManager.MemoryInfo();
                activityManager.GetMemoryInfo(info);

                return info.AvailMem;
            }
        }

        /// <summary>
        /// 返回设备是否正在充电。
        /// </summary>
        [Obsolete]
        public bool IsCharging
        {
            get
            {
                var context = AppUtils.AppContext;
                var intent = context.RegisterReceiver(null, new IntentFilter(Intent.ActionBatteryChanged));
                if (intent == null)
                {
                    return false;
                }

                var plugged = intent.GetIntExtra(BatteryManager.ExtraPlugged, -1);
                return plugged == (int)BatteryManager.BatteryPluggedAc || plugged == (int)BatteryManager.BatteryPluggedUsb;
            }
        }


        public void KeepAwake(WakeLockFlags flags, long timeout)
        {
            CheckWakeLock(flags);
            _mWakeLock.Acquire(timeout);
        }

        public void KeepAwake(WakeLockFlags flags)
        {
            CheckWakeLock(flags);
            _mWakeLock.Acquire();
        }

        /// <summary>
        /// 返回设备屏幕是否是亮着的。如果屏幕亮着，返回true; 否则返回false。
        /// 需要注意的是，类似于vivo xplay系列的息屏时钟不属于"屏幕亮着"的情况，虽然屏幕确实亮着但只能显示时钟而且不可交互，此时 IsScreenOn 也会返回false。
        /// </summary>
        [Obsolete]
        public bool IsScreenOn => GetSystemService<PowerManager>(Context.PowerService).IsScreenOn;

        /// <summary>
        /// 如果屏幕没有点亮，则唤醒设备。
        /// </summary>
        [Obsolete]
        public void WakeUpIfNeeded()
        {
            if (!IsScreenOn)
            {
                WakeUp();
            }
        }

        /// <summary>
        /// 唤醒设备。包括唤醒设备CPU、屏幕等。可以用来点亮屏幕。
        /// </summary>
        public void WakeUp()
        {
            KeepScreenOn(200);
        }

        /// <summary>
        /// 保持屏幕常亮。
        /// 此函数无法阻止用户使用锁屏键等正常关闭屏幕，只能使得设备在无人操作的情况下保持屏幕常亮；同时，如果此函数调用时屏幕没有点亮，则会唤醒屏幕。
        /// 
        /// 在某些设备上，如果不加参数timeout，只能在Auto.js的界面保持屏幕常亮，在其他界面会自动失效，这是因为设备的省电策略造成的。因此，建议使用比较长的时长来代替"一直保持屏幕常亮"的功能，例如device.keepScreenOn(3600 * 1000)。
        /// 
        /// 可以使用device.CancelKeepingAwake()来取消屏幕常亮。
        /// </summary>
        public void KeepScreenOn()
        {
            KeepAwake(WakeLockFlags.ScreenBright | WakeLockFlags.AcquireCausesWakeup);
        }

        /// <summary>
        /// 保持屏幕常亮。
        /// 此函数无法阻止用户使用锁屏键等正常关闭屏幕，只能使得设备在无人操作的情况下保持屏幕常亮；同时，如果此函数调用时屏幕没有点亮，则会唤醒屏幕。
        /// 
        /// 在某些设备上，如果不加参数timeout，只能在Auto.js的界面保持屏幕常亮，在其他界面会自动失效，这是因为设备的省电策略造成的。因此，建议使用比较长的时长来代替"一直保持屏幕常亮"的功能，例如device.keepScreenOn(3600 * 1000)。
        /// 
        /// 可以使用device.CancelKeepingAwake()来取消屏幕常亮。
        /// </summary>
        /// <param name="timeout">屏幕保持常亮的时间, 单位毫秒。如果不加此参数，则一直保持屏幕常亮。</param>
        public void KeepScreenOn(long timeout)
        {
            KeepAwake(WakeLockFlags.ScreenBright | WakeLockFlags.AcquireCausesWakeup, timeout);
        }

        /// <summary>
        /// 保持屏幕常亮，但允许屏幕变暗来节省电量。此函数可以用于定时脚本唤醒屏幕操作，不需要用户观看屏幕，可以让屏幕变暗来节省电量。
        /// 
        /// 此函数无法阻止用户使用锁屏键等正常关闭屏幕，只能使得设备在无人操作的情况下保持屏幕常亮；同时，如果此函数调用时屏幕没有点亮，则会唤醒屏幕。
        /// 
        /// 可以使用device.CancelKeepingAwake()来取消屏幕常亮。
        /// </summary>
        public void KeepScreenDim()
        {
            KeepAwake(WakeLockFlags.ScreenDim | WakeLockFlags.AcquireCausesWakeup);
        }

        /// <summary>
        /// 保持屏幕常亮，但允许屏幕变暗来节省电量。此函数可以用于定时脚本唤醒屏幕操作，不需要用户观看屏幕，可以让屏幕变暗来节省电量。
        /// 
        /// 此函数无法阻止用户使用锁屏键等正常关闭屏幕，只能使得设备在无人操作的情况下保持屏幕常亮；同时，如果此函数调用时屏幕没有点亮，则会唤醒屏幕。
        /// 
        /// 可以使用device.CancelKeepingAwake()来取消屏幕常亮。
        /// </summary>
        /// <param name="timeout">屏幕保持常亮的时间, 单位毫秒。如果不加此参数，则一直保持屏幕常亮。</param>
        public void KeepScreenDim(long timeout)
        {
            KeepAwake(WakeLockFlags.ScreenDim | WakeLockFlags.AcquireCausesWakeup, timeout);
        }

        /// <summary>
        /// 取消设备保持唤醒状态。用于取消device.KeepScreenOn(), device.KeepScreenDim()等函数设置的屏幕常亮。
        /// </summary>
        public void CancelKeepingAwake()
        {
            if (_mWakeLock != null && _mWakeLock.IsHeld)
                _mWakeLock.Release();
        }

        /// <summary>
        /// 使设备震动一段时间。
        /// </summary>
        /// <param name="millis">震动时间，单位毫秒</param>
        [Obsolete]
        public void Vibrate(long millis)
        {
            GetSystemService<Vibrator>(Context.VibratorService).Vibrate(millis);
        }

        /// <summary>
        /// 如果设备处于震动状态，则取消震动。
        /// </summary>
        public void CancelVibration()
        {
            GetSystemService<Vibrator>(Context.VibratorService).Cancel();
        }

        #region private

        private void CheckWakeLock(WakeLockFlags flags)
        {
            if (_mWakeLock != null) return;
            
            CancelKeepingAwake();
            _mWakeLock = GetSystemService<PowerManager>(Context.PowerService).NewWakeLock(flags, Class.FromType(typeof(Device)).Name);
        }

        private static T GetSystemService<T>(string service) where T : class, IJavaObject
        {
            var context = AppUtils.AppContext;
            var systemService = context.GetSystemService(service).JavaCast<T>();
            if (systemService == null)
            {
                throw new RuntimeException("should never happen..." + service);
            }

            return systemService;
        }

        private void CheckWriteSettingsPermission()
        {
            if (CanWriteSettings())
            {
                return;
            }

            if (Build.VERSION.SdkInt < BuildVersionCodes.M) return;
            
            var context = AppUtils.AppContext;

            var intent = new Intent(Settings.ActionManageWriteSettings);

            intent.SetData(Android.Net.Uri.Parse("package:" + context.PackageName));
            context.StartActivity(intent.AddFlags(ActivityFlags.NewTask));

            throw new SecurityException("沒有修改系統设置权限");
        }

        private bool CanWriteSettings()
        {
            var context = AppUtils.AppContext;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                return Settings.System.CanWrite(context);
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr2)
            {
                return CheckOp(context, "23");
            }
            else
            {
                return true;
            }
        }

        private static bool CheckOp(Context context, string op)
        {
            var manager = context.GetSystemService(Context.AppOpsService).JavaCast<AppOpsManager>();
            try
            {
                return manager.CheckOp(op, Binder.CallingUid, context.PackageName) == AppOpsManagerMode.Allowed;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 检测权限
        /// </summary>
        private static void CheckReadPhoneStatePermission()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M) return;
            
            if (AppUtils.AppContext.CheckSelfPermission(Manifest.Permission.ReadPhoneState) != Permission.Granted)
            {
                throw new SecurityException("没有读取设备信息权限");
            }
        }

        private static string GetMacByInterface()
        {
            var networkInterfaces = Collections.List(NetworkInterface.NetworkInterfaces);

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.Name.Equals("wlan0"))
                {
                    var macBytes = networkInterface.GetHardwareAddress();
                    if (macBytes == null)
                        return null;

                    var mac = new StringBuilder();
                    foreach (var b in macBytes)
                    {
                        mac.Append(b);
                    }

                    if (mac.Length() > 0)
                    {
                        mac.DeleteCharAt(mac.Length() - 1);
                    }
                    return mac.ToString();
                }
            }

            return string.Empty;
        }
        private static string GetMacByFile()
        {
            try
            {
                using var f = new System.IO.FileStream("/sys/class/net/wlan0/address", System.IO.FileMode.Open);

                var bytes = new byte[f.Length];
                f.Read(bytes);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }
        #endregion
    }
}