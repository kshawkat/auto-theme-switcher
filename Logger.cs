using System;
using System.Diagnostics;
using System.IO;

namespace AutoThemeSwitcher
{
    public static class Logger
    {
        public static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AutoThemeSwitcher",
            "log.txt"
        );

        private static readonly object lockObj = new object();

        public static void Log(string message)
        {
            try
            {
                lock (lockObj)
                {
                    var directory = Path.GetDirectoryName(LogFilePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var logMessage = $"[{timestamp}] {message}";

                    File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);

                    // Keep log file size manageable (max 1MB)
                    var fileInfo = new FileInfo(LogFilePath);
                    if (fileInfo.Exists && fileInfo.Length > 1024 * 1024)
                    {
                        TruncateLog();
                    }
                }
            }
            catch
            {
                // Silently fail if logging doesn't work
            }
        }

        public static void OpenLog()
        {
            try
            {
                if (File.Exists(LogFilePath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = LogFilePath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(
                        "Log file does not exist yet.",
                        "No Log File",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Error opening log file: {ex.Message}",
                    "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private static void TruncateLog()
        {
            try
            {
                var lines = File.ReadAllLines(LogFilePath);
                var keepLines = lines.Length > 1000 ? lines.Length - 1000 : 0;
                var newLines = new string[lines.Length - keepLines];
                Array.Copy(lines, keepLines, newLines, 0, newLines.Length);
                File.WriteAllLines(LogFilePath, newLines);
            }
            catch
            {
                // If truncation fails, just delete the file
                try { File.Delete(LogFilePath); } catch { }
            }
        }
    }
}
