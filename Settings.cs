using System;
using System.IO;
using Newtonsoft.Json;

namespace AutoThemeSwitcher
{
    public class Settings
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AutoThemeSwitcher",
            "settings.json"
        );

        public double Latitude { get; set; } = 40.063163025405856;
        public double Longitude { get; set; } = -88.24559595778813;
        public bool StartWithWindows { get; set; } = true;
        public bool ShowNotifications { get; set; } = true;

        public static Settings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    return JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error loading settings: {ex.Message}");
            }

            return new Settings();
        }

        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(SettingsPath, json);
                
                Logger.Log("Settings saved successfully");
            }
            catch (Exception ex)
            {
                Logger.Log($"Error saving settings: {ex.Message}");
            }
        }
    }
}