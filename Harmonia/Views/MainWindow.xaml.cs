using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Harmonia.Properties;
using Harmonia.Settings.Interfaces;
using Harmonia.ViewModels;
using Harmonia.Wrappers.Interfaces;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Unity;

namespace Harmonia.Views
{
    public partial class MainWindow : MetroWindow
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IUnityContainer _unityContainer;
        private readonly MainViewModel _viewModel;

        public MainWindow(
            ISettingsProvider settingsProvider,
            IUnityContainer unityContainer,
            MainViewModel mainViewModel,
            IClipboardWrapper clipboardWrapper)
        {
            InitializeComponent();

            _settingsProvider = settingsProvider;
            _unityContainer = unityContainer;
            _viewModel = mainViewModel;
            DataContext = mainViewModel;

            clipboardWrapper.ClipboardTextChanged += ClipboardWrapper_ClipboardTextChanged;
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            await _viewModel.PerformUpdateAsync();
        }

        private async void ClipboardWrapper_ClipboardTextChanged(object sender, string clipboardText)
            => await _viewModel.AddDownloadItem(clipboardText);

        private async void BtnStartAll_Click(object sender, RoutedEventArgs e)
        {
            var isOutputPathValid = IsOutputDirectoryValid();
            if (!isOutputPathValid)
            {
                await ShowMessageAndRedirectToSettings();

                return;
            }

            var isMp3GainPathValid = _settingsProvider.IsMp3GainPathValid();
            if (!isMp3GainPathValid)
            {
                var result = await this.ShowMessageAsync(
                   title: MainResources.MP3GainExecutableNotFound_Title,
                   message: MainResources.MP3GainExecutableNotFound_Message,
                   style: MessageDialogStyle.AffirmativeAndNegative,
                   settings: new MetroDialogSettings
                   {
                       DefaultButtonFocus = MessageDialogResult.Affirmative,
                       AffirmativeButtonText = CommonResources.Yes,
                       NegativeButtonText = CommonResources.No,
                       AnimateHide = false,
                       AnimateShow = true
                   });

                if (result == MessageDialogResult.Affirmative)
                {
                    ShowDialogWindow<SettingsWindow>();
                }
                else
                {
                    await this.ShowMessageAsync(
                        title: MainResources.MP3GainExecutableNotFound_Title,
                        message: MainResources.Mp3GainExecutableNotFound_SkippingAudioNormalization,
                        settings: new MetroDialogSettings
                        {
                            AnimateShow = false
                        });
                }
            }

            await _viewModel.StartDownloads();
        }

        private async void btnOpenOutputDirectory_Click(object sender, RoutedEventArgs e)
        {
            var isOutputPathValid = IsOutputDirectoryValid();
            if (!isOutputPathValid)
            {
                await ShowMessageAndRedirectToSettings();
            }
            else
            {
                Process.Start(_settingsProvider.OutputDirectory);
            }
        }

        private async Task ShowMessageAndRedirectToSettings()
        {
            await this.ShowMessageAsync(
                title: MainResources.OutputPathDoesNotExist_Title,
                message: string.Format(MainResources.OutputPathDoesNotExist_Message, _settingsProvider.OutputDirectory),
                style: MessageDialogStyle.Affirmative
                );

            ShowDialogWindow<SettingsWindow>();
        }

        private bool IsOutputDirectoryValid() => _settingsProvider.IsOutputDirectoryValid();

        private void btnOpenSettings_Click(object sender, RoutedEventArgs e)
            => ShowDialogWindow<SettingsWindow>();

        private void btnOpenAboutWindow_Click(object sender, RoutedEventArgs e)
            => ShowDialogWindow<AboutWindow>();

        private void ShowDialogWindow<T>()
            where T : MetroWindow
        {
            var window = _unityContainer.Resolve<T>();
            window.Owner = this;
            window.ShowDialog();
        }
    }
}