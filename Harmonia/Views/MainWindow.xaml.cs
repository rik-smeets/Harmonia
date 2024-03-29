﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shell;
using Harmonia.Models;
using Harmonia.Properties;
using Harmonia.Settings;
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
        private readonly UserSettings _userSettings;
        private readonly IUnityContainer _unityContainer;
        private readonly MainViewModel _viewModel;
        private readonly IClipboardWrapper _clipboardWrapper;
        private readonly IStorageWrapper _storageWrapper;

        public MainWindow(
            ISettingsManager settingsManager,
            IUnityContainer unityContainer,
            MainViewModel mainViewModel,
            IClipboardWrapper clipboardWrapper,
            IStorageWrapper storageWrapper)
        {
            InitializeComponent();

            _userSettings = settingsManager.LoadSettings();
            _unityContainer = unityContainer;
            _viewModel = mainViewModel;
            _clipboardWrapper = clipboardWrapper;
            _storageWrapper = storageWrapper;
            DataContext = mainViewModel;
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            _clipboardWrapper.ClipboardTextChanged += ClipboardWrapper_ClipboardTextChanged;

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

            var isMp3GainPathValid = _storageWrapper.FileExists(_userSettings.Mp3GainPath);
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

                if (result is MessageDialogResult.Affirmative)
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

            _viewModel.DownloadProgress += (o, downloadProgressArgs) =>
            {
                TaskbarItemInfo.ProgressState = downloadProgressArgs.State;
                TaskbarItemInfo.ProgressValue = downloadProgressArgs.Value;
            };

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
                Process.Start("explorer.exe", _userSettings.OutputDirectory);
            }
        }

        private async Task ShowMessageAndRedirectToSettings()
        {
            await this.ShowMessageAsync(
                title: MainResources.OutputPathDoesNotExist_Title,
                message: string.Format(MainResources.OutputPathDoesNotExist_Message, _userSettings.OutputDirectory),
                style: MessageDialogStyle.Affirmative
                );

            ShowDialogWindow<SettingsWindow>();
        }

        private bool IsOutputDirectoryValid() => _storageWrapper.DirectoryExists(_userSettings.OutputDirectory);

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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is DownloadItem downloadItem)
            {
                _viewModel.DeleteDownloadItem(downloadItem);
            }
        }

        private void dgDownloadItems_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            BindArtistAndTitleOnTextChange(dataGrid);
            DeleteItemsOnCtrlDeletePress(e, dataGrid);
        }

        private void DeleteItemsOnCtrlDeletePress(KeyEventArgs e, DataGrid dataGrid)
        {
            if (e.Key != Key.Delete || !Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                return;
            }

            var selectedDownloadItems = dataGrid.SelectedCells
                .Select(sc => sc.Item)
                .OfType<DownloadItem>()
                .ToArray();

            foreach (var downloadItem in selectedDownloadItems)
            {
                _viewModel.DeleteDownloadItem(downloadItem);
            }
        }

        private static void BindArtistAndTitleOnTextChange(DataGrid datagrid)
        {
            var selectedCells = datagrid.SelectedCells;
            if (selectedCells.Count != 1)
            {
                return;
            }

            var selectedCell = selectedCells[0];
            if (!selectedCell.IsValid ||
                selectedCell.Column is not DataGridTextColumn dgTextColumn ||
                dgTextColumn.Binding is not Binding binding)
            {
                return;
            }

            var downloadItem = (DownloadItem)selectedCell.Item;
            var content = selectedCell.Column.GetCellContent(downloadItem);
            if (content is TextBox textBox)
            {
                switch (binding.Path.Path)
                {
                    case nameof(DownloadItem.Artist):
                        downloadItem.Artist = textBox.Text;
                        break;

                    case nameof(DownloadItem.Title):
                        downloadItem.Title = textBox.Text;
                        break;
                }
            }
        }
    }
}