using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace AutoThemeSwitcher
{
    public class ThemeController
    {
        private const int HWND_BROADCAST = 0xffff;
        private const int WM_SETTINGCHANGE = 0x001a;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            uint Msg,
            UIntPtr wParam,
            string lParam,
            uint fuFlags,
            uint uTimeout,
            out UIntPtr lpdwResult);

        public async Task UpdateThemeBasedOnTime()
        {
            try
            {
                var settings = Settings.Load();
                var sunApi = new SunriseSunsetAPI();
                var sunData = await sunApi.GetSunriseSunsetAsync(settings.Latitude, settings.Longitude);

                if (sunData != null)
                {
                    var now = DateTime.Now;
                    var isDaytime = now >= sunData.Sunrise && now < sunData.Sunset;

                    if (isDaytime)
                    {
                        SetDaytimeTheme();
                    }
                    else
                    {
                        SetNighttimeTheme();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error updating theme: {ex.Message}");
            }
        }

        public static void SetDaytimeTheme()
        {
            // Daytime: System Dark, Apps Light
            SetRegistryValue("SystemUsesLightTheme", 0); // Dark
            SetRegistryValue("AppsUseLightTheme", 1);    // Light
            BroadcastThemeChange();
            Logger.Log("Applied Daytime theme: System Dark, Apps Light");
        }

        public static void SetNighttimeTheme()
        {
            // Nighttime: All Dark
            SetRegistryValue("SystemUsesLightTheme", 0); // Dark
            SetRegistryValue("AppsUseLightTheme", 0);    // Dark
            BroadcastThemeChange();
            Logger.Log("Applied Nighttime theme: All Dark");
        }

        private static void SetRegistryValue(string name, int value)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", true))
                {
                    if (key != null)
                    {
                        key.SetValue(name, value, RegistryValueKind.DWord);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error setting registry value {name}: {ex.Message}");
            }
        }

        private static void BroadcastThemeChange()
        {
            try
            {
                UIntPtr result;
                SendMessageTimeout(
                    (IntPtr)HWND_BROADCAST,
                    WM_SETTINGCHANGE,
                    UIntPtr.Zero,
                    "ImmersiveColorSet",
                    0x0002,
                    5000,
                    out result);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error broadcasting theme change: {ex.Message}");
            }
        }

        public static string GetCurrentThemeMode()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        var systemLight = (int)key.GetValue("SystemUsesLightTheme", 0);
                        var appsLight = (int)key.GetValue("AppsUseLightTheme", 0);
                        
                        if (systemLight == 0 && appsLight == 1)
                            return "Daytime (System Dark, Apps Light)";
                        else if (systemLight == 0 && appsLight == 0)
                            return "Nighttime (All Dark)";
                        else
                            return $"Custom (System {(systemLight == 1 ? "Light" : "Dark")}, Apps {(appsLight == 1 ? "Light" : "Dark")})";
                    }
                }
            }
            catch { }
            return "Unknown";
        }
    }
}
