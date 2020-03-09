using System.Collections.Generic;
using Android.Content;
using Android.Hardware;
using Android.Runtime;
using AutoLua.Core.Common;
using NLua.Exceptions;

namespace AutoLua.Core.LuaScript.Api
{
    /// <summary>
    /// 传感器。
    /// </summary>
    [Preserve(AllMembers = true)]
    public class Sensors
    {
        private static readonly IDictionary<string, SensorType> SENSORS = new Dictionary<string, SensorType>
        {
            {"ACCELEROMETER", SensorType.Accelerometer},
            {"ACCELEROMETER_UNCALIBRATED", SensorType.AccelerometerUncalibrated},
            {"AMBIENT_TEMPERATURE", SensorType.AmbientTemperature},
            {"DEVICE_PRIVATE_BASE", SensorType.DevicePrivateBase},
            {"GAME_ROTATION_VECTOR", SensorType.GameRotationVector},
            {"GEOMAGNETIC_ROTATION_VECTOR", SensorType.GeomagneticRotationVector},
            {"GRAVITY", SensorType.Gravity},
            {"GYROSCOPE", SensorType.Gyroscope},
            {"GYROSCOPE_UNCALIBRATED", SensorType.GyroscopeUncalibrated},
            {"HEART_BEAT", SensorType.HeartBeat},
            {"HEART_RATE", SensorType.HeartRate},
            {"LIGHT", SensorType.Light},
            {"LINEAR_ACCELERATION", SensorType.LinearAcceleration},
            {"LOW_LATENCY_OFFBODY_DETECT", SensorType.LowLatencyOffbodyDetect},
            {"MAGNETIC_FIELD", SensorType.MagneticField},
            {"MAGNETIC_FIELD_UNCALIBRATED", SensorType.MagneticFieldUncalibrated},
            {"MOTION_DETECT", SensorType.MotionDetect},
            {"ORIENTATION", SensorType.Orientation},
            {"POSE_6DOF", SensorType.Pose6dof},
            {"PRESSURE", SensorType.Pressure},
            {"PROXIMITY", SensorType.Proximity},
            {"RELATIVE_HUMIDITY", SensorType.RelativeHumidity},
            {"ROTATION_VECTOR", SensorType.RotationVector},
            {"SIGNIFICANT_MOTION", SensorType.SignificantMotion},
            {"STATIONARY_DETECT", SensorType.StationaryDetect},
            {"STEP_COUNTER", SensorType.StepCounter},
            {"STEP_DETECTOR", SensorType.StepDetector},
            {"TEMPERATURE", SensorType.Temperature},
        };

        private readonly SensorManager _sensorManager;

        public Sensors()
        {
            _sensorManager = AppUtils.GetSystemService<SensorManager>(Context.SensorService);
        }

        public void register(string sensorName, SensorDelay delay = SensorDelay.Normal)
        {
            if (string.IsNullOrWhiteSpace(sensorName))
            {
                throw new LuaException("sensorName = null");
            }

            var sensor = getSensor(sensorName);
            if (sensor == null)
            {
                
            }
        }

        private void register(Sensor sensor, SensorDelay delay)
        {
        }


        private Sensor getSensor(string sensorName)
        {
            if (!SENSORS.ContainsKey(sensorName.ToUpper()))
                return null;

            var type = SENSORS[sensorName];

            return _sensorManager.GetDefaultSensor(type);
        }


        private static class Delay
        {
            public const SensorDelay Normal = SensorDelay.Normal;
            public const SensorDelay Ui = SensorDelay.Ui;
            public const SensorDelay Game = SensorDelay.Game;
            public const SensorDelay Fastest = SensorDelay.Fastest;
        }
    }
}