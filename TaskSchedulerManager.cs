using System;
using System.IO;
using System.Security.Principal;
using Microsoft.Win32.TaskScheduler;

namespace AutoThemeSwitcher
{
    public static class TaskSchedulerManager
    {
        private const string TaskName = "AutoThemeSwitcher_Boot";

        public static void InstallBootTask()
        {
            try
            {
                using (TaskService ts = new TaskService())
                {
                    // Remove existing task if it exists
                    ts.RootFolder.DeleteTask(TaskName, false);

                    // Create a new task definition
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = "Auto Theme Switcher - Runs at logon to set theme based on time";
// td.Principal settings removed to rely on Task Scheduler defaults and avoid MMC error.
	                    // Task will run as the user who created it, which is the default for RegisterTaskDefinition.

// Create a trigger that fires at logon
	                    LogonTrigger logonTrigger = new LogonTrigger
	                    {
	                        UserId = WindowsIdentity.GetCurrent().Name,
	                        Delay = TimeSpan.FromSeconds(10) // Wait 10 seconds after logon
	                    };
	                    td.Triggers.Add(logonTrigger);

                    // Create an action to run the application
                    string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                    td.Actions.Add(new ExecAction(exePath, "/boot", Path.GetDirectoryName(exePath)));

                    // Set additional settings
                    td.Settings.AllowDemandStart = true;
                    td.Settings.AllowHardTerminate = false;
                    td.Settings.DisallowStartIfOnBatteries = false;
                    // td.Settings.RunOnlyIfNetworkAvailable = false; // Removed as it was not respected and may be causing the issue
                    td.Settings.StopIfGoingOnBatteries = false;
                    td.Settings.ExecutionTimeLimit = TimeSpan.FromHours(1);
                    td.Settings.RestartCount = 3;
                    td.Settings.RestartInterval = TimeSpan.FromMinutes(1);

                    // Register the task
                    ts.RootFolder.RegisterTaskDefinition(TaskName, td);

                    Logger.Log("Boot task installed successfully");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error installing boot task: {ex.Message}");
                throw;
            }
        }

        public static void UninstallBootTask()
        {
            try
            {
                using (TaskService ts = new TaskService())
                {
                    ts.RootFolder.DeleteTask(TaskName, false);
                    Logger.Log("Boot task uninstalled successfully");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error uninstalling boot task: {ex.Message}");
                throw;
            }
        }

        public static bool IsBootTaskInstalled()
        {
            try
            {
                using (TaskService ts = new TaskService())
                {
                    var task = ts.GetTask(TaskName);
                    return task != null;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
