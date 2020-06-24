using System.Windows;
using ControlzEx.Theming;
using Harmonia.ViewModels;
using MahApps.Metro.Controls;
using static System.Windows.Forms.DialogResult;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace Harmonia.Views
{
    public partial class SettingsWindow : MetroWindow
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsWindow(SettingsViewModel settingsViewModel)
        {
            InitializeComponent();

            _viewModel = settingsViewModel;
            DataContext = _viewModel;
        }

        private void btnSelectOutputDirectory_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = _viewModel.OutputDirectory
            };

            using (folderBrowserDialog)
            {
                var result = folderBrowserDialog.ShowDialog();
                if (result == OK)
                {
                    _viewModel.OutputDirectory = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void btnSelectMp3GainFolder_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".exe",
                Filter = "EXE Files (*.exe)|*.exe",
                FileName = "mp3gain.exe"
            };

            using (openFileDialog)
            {
                var result = openFileDialog.ShowDialog();
                if (result == OK)
                {
                    _viewModel.Mp3GainPath = openFileDialog.FileName;
                }
            }
        }

        private async void btnMp3GainHelp_Click(object sender, RoutedEventArgs e)
            => await _viewModel.ShowMp3GainHelp();

        private void RadioButtonDark_Checked(object sender, RoutedEventArgs e)
            => _viewModel.SetThemeBaseColor(ThemeManager.BaseColorDarkConst);

        private void RadioButtonLight_Checked(object sender, RoutedEventArgs e)
            => _viewModel.SetThemeBaseColor(ThemeManager.BaseColorLight);
    }
}