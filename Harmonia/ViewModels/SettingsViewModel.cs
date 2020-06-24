using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ControlzEx.Theming;
using Harmonia.Properties;
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

        private readonly ISettingsProvider _settingsProvider;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IProcessWrapper _processWrapper;
        private readonly IThemeManagerWrapper _themeManagerWrapper;

        public SettingsViewModel(
            ISettingsProvider settingsProvider,
            IDialogCoordinator dialogCoordinator,
            IProcessWrapper processWrapper,
            IThemeManagerWrapper themeManagerWrapper)
        {
            _settingsProvider = settingsProvider;
            _dialogCoordinator = dialogCoordinator;
            _processWrapper = processWrapper;
            _themeManagerWrapper = themeManagerWrapper;
        }

        public string OutputDirectory
        {
            get => _settingsProvider.OutputDirectory;
            set => _settingsProvider.OutputDirectory = value;
        }

        public string Mp3GainPath
        {
            get => _settingsProvider.Mp3GainPath;
            set => _settingsProvider.Mp3GainPath = value;
        }

        public void SetThemeBaseColor(string baseColor)
        {
            _themeManagerWrapper.ChangeThemeBaseColor(baseColor);
            _settingsProvider.ThemeBaseColor = baseColor;
        }

        public string SelectedColorScheme
        {
            get => _settingsProvider.ThemeColorScheme;
            set
            {
                _themeManagerWrapper.ChangeThemeColorScheme(value);
                _settingsProvider.ThemeColorScheme = value;
            }
        }

        public async Task ShowMp3GainHelp()
        {
            var isMp3GainPathValid = _settingsProvider.IsMp3GainPathValid();
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

            if (result == MessageDialogResult.Affirmative)
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

        public bool IsLightThemeEnabled => _settingsProvider.ThemeBaseColor == ThemeManager.BaseColorLightConst;
        public bool IsDarkThemeEnabled => _settingsProvider.ThemeBaseColor == ThemeManager.BaseColorDarkConst;
    }
}