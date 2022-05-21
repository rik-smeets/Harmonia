using System.ComponentModel;
using ControlzEx.Theming;
using Harmonia.Properties;
using Harmonia.Settings;
using Harmonia.Settings.Interfaces;
using Harmonia.Wrappers.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using static Harmonia.Properties.ThemeResources;

namespace Harmonia.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static IReadOnlyCollection<string> ThemeColorSchemes { get; } = new[]
        {
            Red, Green, Blue, Purple, Orange, Lime, Emerald, Teal,
            Cyan, Cobalt, Indigo, Violet, Pink, Magenta, Crimson, Amber,
            Yellow, Brown, Olive, Steel, Mauve, Taupe, Sienna
        }.OrderBy(c => c).ToArray();

        private readonly ISettingsManager _settingsManager;
        private readonly UserSettings _userSettings;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IProcessWrapper _processWrapper;
        private readonly IThemeManagerWrapper _themeManagerWrapper;
        private readonly IStorageWrapper _storageWrapper;

        public SettingsViewModel(
            ISettingsManager settingsManager,
            IDialogCoordinator dialogCoordinator,
            IProcessWrapper processWrapper,
            IThemeManagerWrapper themeManagerWrapper,
            IStorageWrapper storageWrapper)
        {
            _settingsManager = settingsManager;
            _userSettings = settingsManager.LoadSettings();
            _dialogCoordinator = dialogCoordinator;
            _processWrapper = processWrapper;
            _themeManagerWrapper = themeManagerWrapper;
            _storageWrapper = storageWrapper;

            PropertyChanged += SettingsViewModel_PropertyChanged;
        }

        private void SettingsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _settingsManager.SaveSettings(_userSettings);
        }

        public string OutputDirectory
        {
            get => _userSettings.OutputDirectory;
            set => _userSettings.OutputDirectory = value;
        }

        public string Mp3GainPath
        {
            get => _userSettings.Mp3GainPath;
            set => _userSettings.Mp3GainPath = value;
        }

        public void SetThemeBaseColor(string baseColor)
        {
            _themeManagerWrapper.ChangeThemeBaseColor(baseColor);
            _userSettings.ThemeBaseColor = baseColor;
        }

        public string SelectedColorScheme
        {
            get => _userSettings.ThemeColorScheme;
            set
            {
                _themeManagerWrapper.ChangeThemeColorScheme(value);
                _userSettings.ThemeColorScheme = value;
            }
        }

        public async Task ShowMp3GainHelp()
        {
            var isMp3GainPathValid = _storageWrapper.FileExists(_userSettings.Mp3GainPath);
            if (isMp3GainPathValid)
            {
                await _dialogCoordinator.ShowMessageAsync(
                   context: this,
                   title: SettingsResources.Mp3GainHelp_Title,
                   message: SettingsResources.Mp3GainHelp_ValidPath);

                return;
            }

            var result = await _dialogCoordinator.ShowMessageAsync(
               context: this,
               title: SettingsResources.Mp3GainHelp_Title,
               message: SettingsResources.Mp3GainHelp_MissingExecutable,
               style: MessageDialogStyle.AffirmativeAndNegative,
               settings: new MetroDialogSettings
               {
                   DefaultButtonFocus = MessageDialogResult.Affirmative,
                   AffirmativeButtonText = CommonResources.Yes,
                   NegativeButtonText = CommonResources.No,
                   AnimateHide = false
               });

            if (result is MessageDialogResult.Affirmative)
            {
                _processWrapper.Start("http://mp3gain.sourceforge.net/");
            }
            else
            {
                await _dialogCoordinator.ShowMessageAsync(
                   context: this,
                   title: SettingsResources.Mp3GainHelp_Title,
                   message: SettingsResources.Mp3GainHelp_SetValidPath,
                   settings: new MetroDialogSettings
                   {
                       AnimateShow = false
                   });
            }
        }

        public bool IsLightThemeEnabled => _userSettings.ThemeBaseColor is ThemeManager.BaseColorLightConst;
        public bool IsDarkThemeEnabled => _userSettings.ThemeBaseColor is ThemeManager.BaseColorDarkConst;
    }
}