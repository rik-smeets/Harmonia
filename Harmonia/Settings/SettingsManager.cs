using System.IO;
using System.Text.Json;
using Harmonia.Settings.Interfaces;

namespace Harmonia.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private readonly string _appSettingsPath;
        private readonly string _appSettingsDirectory;
        private readonly Lazy<UserSettings> _emptyUserSettings = new();

        public SettingsManager()
        {
            _appSettingsDirectory = GetSettingsDirectory();
            _appSettingsPath = Path.Combine(_appSettingsDirectory, "usersettings.json");
            Directory.CreateDirectory(_appSettingsDirectory);
        }

        public UserSettings LoadSettings() =>
            File.Exists(_appSettingsPath) 
            ? JsonSerializer.Deserialize<UserSettings>(File.ReadAllText(_appSettingsPath)) 
            : _emptyUserSettings.Value;

        public void SaveSettings(UserSettings settings) => 
            File.WriteAllText(_appSettingsPath, JsonSerializer.Serialize(settings));

        private static string GetSettingsDirectory()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "Harmonia");
        }
    }
}
