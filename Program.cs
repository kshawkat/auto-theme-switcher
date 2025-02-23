using System;
using System.Windows.Forms;

namespace AutoThemeSwitcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Check if already running
            bool createdNew;
            using (var mutex = new System.Threading.Mutex(true, "AutoThemeSwitcher_SingleInstance", out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show("Auto Theme Switcher is already running!", 
                        "Already Running", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                // Check for command line arguments
                if (args.Length > 0)
                {
                    HandleCommandLineArgs(args);
                }
                else
                {
                    // Run as tray application
                    Application.Run(new TrayApplicationContext());
                }
            }
        }

        static void HandleCommandLineArgs(string[] args)
        {
            var command = args[0].ToLower();
            
            switch (command)
            {
                case "/install":
                    TaskSchedulerManager.InstallBootTask();
                    MessageBox.Show("Boot task installed successfully!", 
                        "Installation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                    
                case "/uninstall":
                    TaskSchedulerManager.UninstallBootTask();
                    MessageBox.Show("Boot task uninstalled successfully!", 
                        "Uninstallation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                    
                case "/boot":
                    {
                        // Run once and exit after boot/logon
                        var controller = new ThemeController();
                        controller.UpdateThemeBasedOnTime().Wait();
                        break;
                    }
                case "/once":
                    // Run once and exit
                    new ThemeController().UpdateThemeBasedOnTime().Wait();
                    break;
                    
                default:
                    MessageBox.Show($"Unknown command: {command}\\n\\nAvailable commands:\\n/install - Install boot task\\n/uninstall - Remove boot task\\n/boot - Run once and exit after boot/logon\n/once - Run once and exit", 
                        "Invalid Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }
    }
}