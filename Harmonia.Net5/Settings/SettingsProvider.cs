using System.IO;
using ControlzEx.Theming;
using Harmonia.Net5.Properties;
using Harmonia.Net5.Settings;
using Harmonia.Settings.Interfaces;
using Harmonia.Wrappers.Interfaces;
using static System.Environment;

namespace Harmonia.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        private readonly IStorageWrapper _storageWrapper;
        private readonly SettingsRetriever _settingsRetriever = SettingsRetriever.Default;

        public SettingsProvider(IStorageWrapper storageWrapper)
        {
            _storageWrapper = storageWrapper;
        }

        public string OutputDirectory
        {
            get => !string.IsNullOrEmpty(_settingsRetriever.OutputPath)
                ? _settingsRetriever.OutputPath
                : GetFolderPath(SpecialFolder.MyMusic);
            set => SettingsRetriever.Default.OutputPath = value;
        }

        public string Mp3GainPath
        {
            get => !string.IsNullOrEmpty(_settingsRetriever.Mp3GainPath)
                ? _settingsRetriever.Mp3GainPath
                : Path.Combine(GetFolderPath(SpecialFolder.ProgramFilesX86), "MP3Gain", "mp3gain.exe");
            set => _settingsRetriever.Mp3GainPath = value;
        }

        public string ThemeBaseColor
        {
            get => !string.IsNullOrEmpty(_settingsRetriever.ThemeBaseColor)
                ? _settingsRetriever.ThemeBaseColor
                : ThemeManager.BaseColorDarkConst;
            set => _settingsRetriever.ThemeBaseColor = value;
        }

        public string ThemeColorScheme
        {
            get => !string.IsNullOrEmpty(_settingsRetriever.ThemeColorScheme)
                ? _settingsRetriever.ThemeColorScheme
                : ThemeResources.Olive;
            set => _settingsRetriever.ThemeColorScheme = value;
        }

        public bool IsOutputDirectoryValid() => _storageWrapper.DirectoryExists(OutputDirectory);

        public bool IsMp3GainPathValid() => _storageWrapper.FileExists(Mp3GainPath);

        public void UpgradeIfRequired()
        {
            if (_settingsRetriever.IsUpgradeRequired)
            {
                _settingsRetriever.Upgrade();
                _settingsRetriever.IsUpgradeRequired = false;
            }
        }
    }
}