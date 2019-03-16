using System;
using System.Management;

namespace Tower.Services.WmiBrightness
{
    internal static class WmiHelper
    {
        private static readonly ManagementScope ManagementScope;
        private static readonly SelectQuery WmiBrightnessQuery;
        private static readonly SelectQuery WmiBrightnessMethodsQuery;

        static WmiHelper()
        {
            ManagementScope = new ManagementScope("root\\WMI");
            WmiBrightnessQuery = new SelectQuery("WmiMonitorBrightness");
            WmiBrightnessMethodsQuery = new SelectQuery("WmiMonitorBrightnessMethods");
        }

        internal static bool CanBeUsed()
        {
            using (var managementSearcher = new ManagementObjectSearcher(ManagementScope, WmiBrightnessQuery))
            using (ManagementObjectCollection managementCollection = managementSearcher.Get())
            {
                try
                {
                    var levels = (byte[])GetManagementObjectFromCollection(managementCollection).GetPropertyValue("Level");
                    // Display device's state may allow querying, but may not expose any valid levels
                    return levels.Length > 0;
                }
                catch (InvalidOperationException)
                {
                    // Display device doesn't support setting the brightness
                    return false;
                }
                catch (ManagementException)
                {
                    // Display device doesn't support setting the brightness
                    return false;
                }
            }
        }

        internal static void SetBrightness(double brightnessPercent)
        {
            // Clamp brightness between 0 and 100
            var brightness = (byte) Math.Max(0, Math.Min(100, brightnessPercent * 100));
            SetBrightness(brightness);
        }

        private static ManagementObject GetManagementObjectFromCollection(ManagementObjectCollection collection)
        {
            ManagementObjectCollection.ManagementObjectEnumerator collectionEnumerator = collection.GetEnumerator();
            collectionEnumerator.MoveNext();
            return (ManagementObject)collectionEnumerator.Current;
        }

        private static void SetBrightness(byte brightness)
        {
            using (var managementSearcher = new ManagementObjectSearcher(ManagementScope, WmiBrightnessMethodsQuery))
            using (ManagementObjectCollection managementCollection = managementSearcher.Get())
            {
                ManagementObject managementObject = GetManagementObjectFromCollection(managementCollection);
                managementObject.InvokeMethod("WmiSetBrightness", new object[] { uint.MaxValue, brightness });
            }
        }
    }
}
