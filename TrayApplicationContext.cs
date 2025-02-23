using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoThemeSwitcher
{
    public class TrayApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private Timer checkTimer;
        private DateTime lastSunrise;
        private DateTime lastSunset;
        private bool isDaytime;
        private SunriseSunsetAPI sunApi;

        public TrayApplicationContext()
        {
            sunApi = new SunriseSunsetAPI();
            InitializeTrayIcon();
            InitializeTimer();
            CheckAndUpdateTheme();
        }

        private void InitializeTrayIcon()
        {
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Check Now", null, OnCheckNow);
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Switch to Daytime Theme", null, OnSwitchToDaytime);
            contextMenu.Items.Add("Switch to Nighttime Theme", null, OnSwitchToNighttime);
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Settings", null, OnSettings);
            contextMenu.Items.Add("View Log", null, OnViewLog);
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Install Boot Task", null, OnInstallBootTask);
            contextMenu.Items.Add("Uninstall Boot Task", null, OnUninstallBootTask);
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Exit", null, OnExit);

            trayIcon = new NotifyIcon
            {
                ContextMenuStrip = contextMenu,
                Visible = true,
                Text = "Auto Theme Switcher"
            };

            UpdateTrayIcon(true); // Start with daytime icon
        }

private void UpdateTrayIcon(bool daytime)
{
    isDaytime = daytime;
    
    // Create dynamic icon with 40% larger size (32px -> 44px)
    using (Bitmap bmp = new Bitmap(44, 44)) // Scaled from 32x32 to 44x44
    using (Graphics g = Graphics.FromImage(bmp))
    {
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.Transparent);

        if (daytime)
        {
            // Sun icon for daytime
            using (Brush sunBrush = new SolidBrush(Color.FromArgb(255, 200, 0)))
            {
                // Sun circle (scaled: 8->11.2, 16->22.4)
                g.FillEllipse(sunBrush, 11, 11, 22, 22);
                
                // Sun rays (scaled: pen width 2->2.8, distances adjusted)
                using (Pen rayPen = new Pen(Color.FromArgb(255, 200, 0), 2.8f))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        double angle = i * Math.PI / 4;
                        // Center at 22 (44/2), scaled ray lengths: 12->16.8, 15->21
                        int x1 = 22 + (int)(Math.Cos(angle) * 16.8);
                        int y1 = 22 + (int)(Math.Sin(angle) * 16.8);
                        int x2 = 22 + (int)(Math.Cos(angle) * 21);
                        int y2 = 22 + (int)(Math.Sin(angle) * 21);
                        g.DrawLine(rayPen, x1, y1, x2, y2);
                    }
                }
            }
        }
        else
        {
            // Moon icon for nighttime
            using (Brush moonBrush = new SolidBrush(Color.FromArgb(200, 200, 220)))
            using (Brush shadowBrush = new SolidBrush(Color.FromArgb(30, 30, 50)))
            {
                // Moon circle (scaled: 6->8.4, 20->28)
                g.FillEllipse(moonBrush, 8, 8, 28, 28);
                
                // Moon shadow (crescent effect, scaled: 12->16.8, 20->28)
                g.FillEllipse(shadowBrush, 17, 8, 28, 28);
            }
        }

        trayIcon.Icon = Icon.FromHandle(bmp.GetHicon());
    }

    trayIcon.Text = $"Auto Theme Switcher - {(daytime ? "Daytime" : "Nighttime")} Mode";
}

        private void InitializeTimer()
        {
            checkTimer = new Timer { Interval = 60000 }; // Check every 60 seconds
            checkTimer.Tick += (s, e) => CheckAndUpdateTheme();
            checkTimer.Start();
        }

        private async void CheckAndUpdateTheme()
        {
            try
            {
                var settings = Settings.Load();
                var sunData = await sunApi.GetSunriseSunsetAsync(
                    settings.Latitude, settings.Longitude);

                if (sunData != null)
                {
                    lastSunrise = sunData.Sunrise;
                    lastSunset = sunData.Sunset;

                    var now = DateTime.Now;
                    var shouldBeDaytime = now >= sunData.Sunrise && now < sunData.Sunset;

                    if (shouldBeDaytime != isDaytime)
                    {
                        if (shouldBeDaytime)
                        {
                            ThemeController.SetDaytimeTheme();
                            UpdateTrayIcon(true);
                            Logger.Log($"Switched to Daytime theme at {now}");
                        }
                        else
                        {
                            ThemeController.SetNighttimeTheme();
                            UpdateTrayIcon(false);
                            Logger.Log($"Switched to Nighttime theme at {now}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error in CheckAndUpdateTheme: {ex.Message}");
            }
        }

        private void OnCheckNow(object sender, EventArgs e)
        {
            CheckAndUpdateTheme();
            MessageBox.Show($"Current theme: {ThemeController.GetCurrentThemeMode()}\n" +
                          $"Last Sunrise: {lastSunrise:HH:mm}\n" +
                          $"Last Sunset: {lastSunset:HH:mm}",
                          "Theme Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnSwitchToDaytime(object sender, EventArgs e)
        {
            ThemeController.SetDaytimeTheme();
            UpdateTrayIcon(true);
            MessageBox.Show("Switched to Daytime theme (System Dark, Apps Light)",
                          "Theme Changed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnSwitchToNighttime(object sender, EventArgs e)
        {
            ThemeController.SetNighttimeTheme();
            UpdateTrayIcon(false);
            MessageBox.Show("Switched to Nighttime theme (All Dark)",
                          "Theme Changed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnSettings(object sender, EventArgs e)
        {
            var currentSettings = Settings.Load();
            var settingsForm = new SettingsForm(currentSettings);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                settingsForm.Settings.Save();
                // Optionally trigger an immediate theme check
                CheckAndUpdateTheme();
            }
        }

        private void OnViewLog(object sender, EventArgs e)
        {
            Logger.OpenLog();
        }

        private void OnInstallBootTask(object sender, EventArgs e)
        {
            try
            {
                TaskSchedulerManager.InstallBootTask();
                MessageBox.Show("Boot task installed successfully!",
                              "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to install boot task: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnUninstallBootTask(object sender, EventArgs e)
        {
            try
            {
                TaskSchedulerManager.UninstallBootTask();
                MessageBox.Show("Boot task uninstalled successfully!",
                              "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to uninstall boot task: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            checkTimer?.Stop();
            checkTimer?.Dispose();
            trayIcon?.Dispose();
            Application.Exit();
        }
    }
}
