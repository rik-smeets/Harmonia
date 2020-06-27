using System.IO;
using ControlzEx.Theming;
using Harmonia.Properties;
using Harmonia.Settings.Interfaces;
using Harmonia.Wrappers.Interfaces;
using static System.Environment;

namespace Harmonia.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        private readonly ISettingsRetriever _settingsRetriever;
        private readonly IStorageWrapper _storageWrapper;

        public SettingsProvider(ISettingsRetriever settingsRetriever, IStorageWrapper storageWrapper)
        {
            _settingsRetriever = settingsRetriever;
            _storageWrapper = storageWrapper;
        }

        public string OutputDirectory
        {
            get => !string.IsNullOrEmpty(_settingsRetriever.OutputPath)
                ? _settingsRetriever.OutputPath
                : GetFolderPath(SpecialFolder.MyMusic);
            set => _settingsRetriever.OutputPath = value;
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
                _settingsRetriever.PerformUpgrade();
                _settingsRetriever.IsUpgradeRequired = false;
            }
        }
    }
}