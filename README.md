# Auto Theme Switcher for Windows 11

A lightweight, single-file system tray application that automatically switches between Light and Dark themes based on local sunrise and sunset times. No Explorer restarts required—seamless transitions via Windows API.

## Features

- ✅ **Automatic Theme Switching**: Switches themes at sunrise (to Light) and sunset (to Dark).
- ✅ **Single-File Executable**: Builds as a standalone `.exe` with all dependencies embedded—no DLLs needed.
- ✅ **Dynamic Tray Icon**: Sun icon (yellow/orange) during the day; moon icon (gray/blue) at night. Tooltip shows current mode.
- ✅ **Seamless Updates**: Uses Windows API and broadcasts `WM_SETTINGCHANGE` for instant app notifications without restarting Explorer.
- ✅ **Boot Task Integration**: Starts automatically with Windows via scheduled task.
- ✅ **Configurable Location**: Enter latitude/longitude for precise sunrise/sunset calculations (fetched from [sunrise-sunset.org](https://sunrise-sunset.org/api)).
- ✅ **Manual Controls**: Right-click menu for overrides, settings, logging, and more.
- ✅ **Logging**: Detailed logs for easy troubleshooting.

**Theme Behavior**:
- **Daytime**: System Dark + Apps Light (customizable in `ThemeController.cs`, line 55: change to "System Light" if preferred).
- **Nighttime**: All Dark.
- Properly configures Windows 11 registry values for consistent results.

## Quick Start

### Prerequisites
- Windows 11 (or Windows 10 with theme support).
- [.NET 9.0 Runtime](https://dotnet.microsoft.com/download/dotnet/9.0) or later.
- Internet connection (for initial sunrise/sunset data fetch).

### Installation
1. Download the latest release from the [Releases page](https://github.com/kshawkat/auto-theme-switcher/releases).
2. Extract & save the program to a permanent location (e.g., `%ProgramData%\ThemeSwitcher\AutoThemeSwitcher.exe`)
2. Run `AutoThemeSwitcher.exe` as an administrator.
3. The app will appear in your system tray (look for the sun/moon icon).
4. Right-click the icon → **Settings**:
   - Enter your latitude and longitude (find them at [latlong.net](https://www.latlong.net/)).
   - Optionally enable **Start with Windows**.
   - Click **OK** to save.

### System Tray Menu
Right-click the tray icon for quick actions:

| Option              | Description |
|---------------------|-------------|
| **Check Now**       | Immediately fetch times and update theme. |
| **Switch to Light** | Manually set daytime theme. |
| **Switch to Dark**  | Manually set nighttime theme. |
| **Settings**        | Configure location, startup, and preferences. |
| **View Log**        | Open `log.txt` in Notepad. |
| **Install Boot Task** | Enable auto-start on Windows boot. |
| **Uninstall Boot Task** | Disable auto-start. |
| **Exit**            | Close the application. |

### Command-Line Options
Run from Command Prompt or PowerShell for automation:

- `AutoThemeSwitcher.exe /install` – Install boot task for auto-start.
- `AutoThemeSwitcher.exe /uninstall` – Remove boot task.
- `AutoThemeSwitcher.exe /once` – Run once (fetch and switch), then exit (great for testing).

## How It Works

1. **Launch**: Starts minimized to tray and performs an initial time check.
2. **API Fetch**: Queries [sunrise-sunset.org](https://sunrise-sunset.org/api) for today's sunrise/sunset based on your coordinates.
3. **Theme Validation**: Compares current system theme against expected (Light for day, Dark for night).
4. **Apply Changes**: Updates registry and broadcasts system message if needed—no restarts.
5. **Periodic Monitoring**: Re-checks every 60 seconds; full API refresh daily at midnight.
6. **Icon & Tooltip Update**: Tray icon and status reflect the current mode.

The app runs efficiently in the background, using minimal CPU/RAM.

## Configuration & Logs

- **Settings File**: `%APPDATA%\AutoThemeSwitcher\settings.json` (JSON format for easy editing).
- **Log File**: `%APPDATA%\AutoThemeSwitcher\log.txt` (appends timestamps and events).

Example `settings.json`:
```json
{
  "Latitude": 40.7128,
  "Longitude": -74.0060,
  "StartWithWindows": true,
  "CheckIntervalMinutes": 60
}
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| **Theme doesn't switch automatically** | - Check `log.txt` for API errors.<br>- Verify coordinates and internet access.<br>- Run **Check Now** manually. |
| **App doesn't start on boot** | - Right-click → **Install Boot Task**.<br>- Or use `/install` command.<br>- Ensure Task Scheduler allows it (run as admin if needed). |
| **UI doesn't update after switch** | - Normal for legacy apps; close/reopen them.<br>- Modern UWP/WinUI apps update instantly.<br>- Restart Explorer via Task Manager if persistent. |
| **Icon stuck or wrong mode** | - Run **Check Now** or restart the app.<br>- Confirm system time/date is accurate. |

If issues persist, check the logs and [open an issue](https://github.com/kshawkat/auto-theme-switcher/issues) with details.

## Privacy & Security

- **Minimal Data**: Only fetches anonymous sunrise/sunset data—no personal info sent.
- **No Telemetry**: Zero tracking or phoning home beyond the API.
- **Local Storage**: All settings/logs stored on your machine.
- **Open Source**: Review the code for peace of mind.

## Building from Source

1. Clone the repo: `https://github.com/kshawkat/auto-theme-switcher.git`.
2. Open in Visual Studio 2022+.
3. Set to Release mode and publish as single-file (self-contained).
4. Requires .NET 9.0 SDK.

## License

MIT License – Free for personal and commercial use. See [LICENSE](LICENSE) for details.

---

⭐ Star the repo if it helps! Contributions welcome via pull requests. Questions? [Open an issue](https://github.com/kshawkat/auto-theme-switcher/issues).

