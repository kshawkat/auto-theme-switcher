using System;
using System.Windows.Forms;

namespace AutoThemeSwitcher
{
    public partial class SettingsForm : Form
    {
        public Settings Settings { get; private set; }

        public SettingsForm(Settings currentSettings)
        {
            Settings = currentSettings;
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Settings - Auto Theme Switcher";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 250);

            // Latitude
            var lblLatitude = new Label
            {
                Text = "Latitude:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblLatitude);

            txtLatitude = new TextBox
            {
                Location = new System.Drawing.Point(120, 17),
                Width = 240
            };
            this.Controls.Add(txtLatitude);

            // Longitude
            var lblLongitude = new Label
            {
                Text = "Longitude:",
                Location = new System.Drawing.Point(20, 50),
                AutoSize = true
            };
            this.Controls.Add(lblLongitude);

            txtLongitude = new TextBox
            {
                Location = new System.Drawing.Point(120, 47),
                Width = 240
            };
            this.Controls.Add(txtLongitude);

            // Start with Windows
            chkStartWithWindows = new CheckBox
            {
                Text = "Start with Windows (Install Boot Task)",
                Location = new System.Drawing.Point(20, 80),
                AutoSize = true,
                Width = 340
            };
            this.Controls.Add(chkStartWithWindows);

            // Show notifications
            chkShowNotifications = new CheckBox
            {
                Text = "Show notifications when theme changes",
                Location = new System.Drawing.Point(20, 110),
                AutoSize = true,
                Width = 340
            };
            this.Controls.Add(chkShowNotifications);

            // Help text
            var lblHelp = new Label
            {
                Text = "You can find your coordinates at: https://www.latlong.net/",
                Location = new System.Drawing.Point(20, 140),
                AutoSize = true,
                ForeColor = System.Drawing.Color.Gray
            };
            this.Controls.Add(lblHelp);

            // OK Button
            var btnOK = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(200, 175),
                Width = 75
            };
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            // Cancel Button
            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(285, 175),
                Width = 75
            };
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private TextBox txtLatitude;
        private TextBox txtLongitude;
        private CheckBox chkStartWithWindows;
        private CheckBox chkShowNotifications;

        private void LoadSettings()
        {
            txtLatitude.Text = Settings.Latitude.ToString();
            txtLongitude.Text = Settings.Longitude.ToString();
            chkStartWithWindows.Checked = Settings.StartWithWindows;
            chkShowNotifications.Checked = Settings.ShowNotifications;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.Latitude = double.Parse(txtLatitude.Text);
                Settings.Longitude = double.Parse(txtLongitude.Text);
                Settings.StartWithWindows = chkStartWithWindows.Checked;
                Settings.ShowNotifications = chkShowNotifications.Checked;

                // Handle boot task based on setting
                if (Settings.StartWithWindows && !TaskSchedulerManager.IsBootTaskInstalled())
                {
                    TaskSchedulerManager.InstallBootTask();
                }
                else if (!Settings.StartWithWindows && TaskSchedulerManager.IsBootTaskInstalled())
                {
                    TaskSchedulerManager.UninstallBootTask();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid settings: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
    }
}