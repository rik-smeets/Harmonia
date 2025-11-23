using Harmonia.Properties;
using Harmonia.Settings;
using Harmonia.Settings.Interfaces;
using Harmonia.ViewModels;
using Harmonia.Wrappers.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace Harmonia.UnitTests.ViewModels
{
    [TestClass]
    public class SettingsViewModelTests
    {
        private readonly UserSettings _userSettings = new() { Mp3GainPath = "mp3gain.exe" };
        private MockRepository _mockRepository;
        private Mock<ISettingsManager> _settingsManagerMock;
        private Mock<IDialogCoordinator> _dialogCoordinatorMock;
        private Mock<IProcessWrapper> _processWrapperMock;
        private Mock<IThemeManagerWrapper> _themeManagerWrapperMock;
        private Mock<IStorageWrapper> _storageWrapperMock;
        private SettingsViewModel _settingsViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _settingsManagerMock = _mockRepository.Create<ISettingsManager>();
            _settingsManagerMock.Setup(m => m.LoadSettings()).Returns(_userSettings);
            _dialogCoordinatorMock = _mockRepository.Create<IDialogCoordinator>();
            _processWrapperMock = _mockRepository.Create<IProcessWrapper>();
            _themeManagerWrapperMock = _mockRepository.Create<IThemeManagerWrapper>();
            _storageWrapperMock = _mockRepository.Create<IStorageWrapper>();

            _settingsViewModel = new SettingsViewModel(
                _settingsManagerMock.Object,
                _dialogCoordinatorMock.Object,
                _processWrapperMock.Object,
                _themeManagerWrapperMock.Object,
                _storageWrapperMock.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockRepository.VerifyAll();
            _mockRepository.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void OutputDirectory_WhenGetIsCalled_ThenReturnsSettingsProviderValue()
        {
            const string expectedOutputDirectory = @"C:\Temp";
            _userSettings.OutputDirectory = expectedOutputDirectory;

            var result = _settingsViewModel.OutputDirectory;

            result.ShouldBe(expectedOutputDirectory);
        }

        [TestMethod]
        public void OutputDirectory_WhenSetIsCalled_ThenSetsValueOnSettingsProvider()
        {
            _settingsManagerMock.Setup(x => x.SaveSettings(_userSettings));

            const string expectedOutputDirectory = @"C:\Temp";
            _settingsViewModel.OutputDirectory = expectedOutputDirectory;

            _userSettings.OutputDirectory.ShouldBe(expectedOutputDirectory);
        }

        [TestMethod]
        public void Mp3GainPath_WhenGetIsCalled_ThenReturnsSettingsProviderValue()
        {
            const string expectedPath = @"C:\Temp\mp3gain.exe";
            _userSettings.Mp3GainPath = expectedPath;

            var result = _settingsViewModel.Mp3GainPath;
            
            result.ShouldBe(expectedPath);
        }

        [TestMethod]
        public void Mp3GainPath_WhenSetIsCalled_ThenSetsValueOnSettingsProvider()
        {
            const string expectedMp3GainPath = @"C:\Temp\mp3gain.exe";
            _settingsManagerMock.Setup(x => x.SaveSettings(_userSettings));

            _settingsViewModel.Mp3GainPath = expectedMp3GainPath;

            _userSettings.Mp3GainPath.ShouldBe(expectedMp3GainPath);
        }

        [TestMethod]
        public async Task ShowMp3GainHelp_WhenCalledWithValidPath_ThenOnlyShowsInformationalMessage()
        {
            _dialogCoordinatorMock
                .Setup(d => d.ShowMessageAsync(
                    _settingsViewModel,
                    SettingsResources.Mp3GainHelp_Title,
                    SettingsResources.Mp3GainHelp_ValidPath,
                    MessageDialogStyle.Affirmative,
                    null))
                .ReturnsAsync(MessageDialogResult.Affirmative);

            _storageWrapperMock.Setup(s => s.FileExists(_userSettings.Mp3GainPath))
                .Returns(true);

            await _settingsViewModel.ShowMp3GainHelp();
        }

        [TestMethod]
        public async Task ShowMp3GainHelp_WhenPathIsInvalidAndUserChoosesAffirmative_ThenOpensMp3GainSite()
        {
            _storageWrapperMock.Setup(s => s.FileExists(_userSettings.Mp3GainPath))
                .Returns(false);

            _dialogCoordinatorMock
                .Setup(d => d.ShowMessageAsync(
                    _settingsViewModel,
                    SettingsResources.Mp3GainHelp_Title,
                    SettingsResources.Mp3GainHelp_MissingExecutable,
                    MessageDialogStyle.AffirmativeAndNegative,
                    It.IsAny<MetroDialogSettings>()))
                .ReturnsAsync(MessageDialogResult.Affirmative);

            _processWrapperMock
                .Setup(p => p.Start("http://mp3gain.sourceforge.net/"));

            await _settingsViewModel.ShowMp3GainHelp();
        }

        [TestMethod]
        [DataRow(MessageDialogResult.Canceled)]
        [DataRow(MessageDialogResult.Negative)]
        [DataRow(MessageDialogResult.FirstAuxiliary)]
        [DataRow(MessageDialogResult.SecondAuxiliary)]
        public async Task ShowMp3GainHelp_WhenPathIsInvalidAndUserDoesNotChooseAffirmative_ThenShowsTwoMessages(
            MessageDialogResult dialogResult)
        {
            _storageWrapperMock.Setup(s => s.FileExists(_userSettings.Mp3GainPath))
                .Returns(false);

            _dialogCoordinatorMock
                .Setup(d => d.ShowMessageAsync(
                    _settingsViewModel,
                    SettingsResources.Mp3GainHelp_Title,
                    SettingsResources.Mp3GainHelp_MissingExecutable,
                    MessageDialogStyle.AffirmativeAndNegative,
                    It.Is<MetroDialogSettings>(
                        s => s.DefaultButtonFocus == MessageDialogResult.Affirmative &&
                        s.AffirmativeButtonText == CommonResources.Yes &&
                        s.NegativeButtonText == CommonResources.No &&
                        s.AnimateHide == false)))
                .ReturnsAsync(dialogResult);

            _dialogCoordinatorMock
                .Setup(d => d.ShowMessageAsync(
                   _settingsViewModel,
                   SettingsResources.Mp3GainHelp_Title,
                   SettingsResources.Mp3GainHelp_SetValidPath,
                   MessageDialogStyle.Affirmative,
                   It.Is<MetroDialogSettings>(s => s.AnimateShow == false)))
                .ReturnsAsync(MessageDialogResult.Affirmative);

            await _settingsViewModel.ShowMp3GainHelp();
        }

        [TestMethod]
        public void SetThemeBaseColor_WhenCalled_ThenCallsThemeManagerWrapper()
        {
            const string expectedThemeBaseColor = "Dark";

            _themeManagerWrapperMock
                .Setup(m => m.ChangeThemeBaseColor(expectedThemeBaseColor));

            _settingsViewModel.SetThemeBaseColor(expectedThemeBaseColor);
            _userSettings.ThemeBaseColor.ShouldBe(expectedThemeBaseColor);
        }

        [TestMethod]
        [DataRow("Dark", true)]
        [DataRow("Light", false)]
        public void IsDarkThemeEnabled_WhenCalled_ThenReturnsBoolBasedOnSettingsProviderResult(
            string themeBaseColor, bool expectedResult)
        {
            _userSettings.ThemeBaseColor = themeBaseColor;

            var result = _settingsViewModel.IsDarkThemeEnabled;

            result.ShouldBe(expectedResult);
        }

        [TestMethod]
        [DataRow("Dark", false)]
        [DataRow("Light", true)]
        public void IsLightThemeEnabled_WhenCalled_ThenReturnsBoolBasedOnSettingsProviderResult(
            string themeBaseColor, bool expectedResult)
        {
            _userSettings.ThemeBaseColor = themeBaseColor;

            var result = _settingsViewModel.IsLightThemeEnabled;

            result.ShouldBe(expectedResult);
        }

        [TestMethod]
        public void ThemeColorSchemes_WhenCalled_ThenReturnsColorSchemesAlphabetically()
        {
            var themeColorSchemes = SettingsViewModel.ThemeColorSchemes;

            themeColorSchemes.Count.ShouldBe(23);
            themeColorSchemes.First().ShouldBe(ThemeResources.Amber);
            themeColorSchemes.Last().ShouldBe(ThemeResources.Yellow);
        }

        [TestMethod]
        public void SelectedColorScheme_WhenGetIsCalled_ThenReturnsSettingsProviderValue()
        {
            var expectedColorScheme = ThemeResources.Cyan;
            _userSettings.ThemeColorScheme = expectedColorScheme;

            var result = _settingsViewModel.SelectedColorScheme;

            result.ShouldBe(expectedColorScheme);
        }

        [TestMethod]
        public void SelectedColorScheme_WhenSetIsCalled_ThenSetsValueOnSettingsProviderAndChangesColorScheme()
        {
            _settingsManagerMock.Setup(x => x.SaveSettings(_userSettings));
            var expectedColorScheme = ThemeResources.Green;
            _themeManagerWrapperMock
                .Setup(m => m.ChangeThemeColorScheme(expectedColorScheme));

            _settingsViewModel.SelectedColorScheme = expectedColorScheme;
            _userSettings.ThemeColorScheme.ShouldBe(expectedColorScheme);
        }
    }
}