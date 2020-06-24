using Harmonia.Properties;
using Harmonia.Settings;
using Harmonia.Settings.Interfaces;
using Harmonia.Wrappers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using static System.Environment;

namespace Harmonia.UnitTests.ViewModels
{
    [TestClass]
    public class SettingsProviderTests
    {
        private MockRepository _mockRepository;
        private Mock<ISettingsRetriever> _settingsRetrieverMock;
        private Mock<IStorageWrapper> _storageWrapperMock;
        private SettingsProvider _settingsProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _settingsRetrieverMock = _mockRepository.Create<ISettingsRetriever>();
            _storageWrapperMock = _mockRepository.Create<IStorageWrapper>();

            _settingsProvider = new SettingsProvider(_settingsRetrieverMock.Object, _storageWrapperMock.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockRepository.VerifyAll();
            _mockRepository.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void OutputDirectory_WhenCalled_GetsOutputPathFromSettingsRetriever()
        {
            const string expectedOutputPath = "C:\\Test";

            _settingsRetrieverMock
                .Setup(m => m.OutputPath)
                .Returns(expectedOutputPath);

            var result = _settingsProvider.OutputDirectory;

            result.ShouldBe(expectedOutputPath);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void OutputDirectory_WhenCalledButSettingsRetrieverReturnsNullOrEmpty_ThenReturnsMusicFolder(string value)
        {
            _settingsRetrieverMock
                .Setup(m => m.OutputPath)
                .Returns(value);

            var result = _settingsProvider.OutputDirectory;

            result.ShouldBe(GetFolderPath(SpecialFolder.MyMusic));
        }

        [TestMethod]
        public void Mp3GainPath_WhenCalled_ThenReturnsMp3GainPathFromSettingsRetriever()
        {
            const string mp3GainPath = "C:\\Program Files (x86)\\MP3Gain\\mp3gain.exe";

            _settingsRetrieverMock
                .Setup(m => m.Mp3GainPath)
                .Returns(mp3GainPath);

            var result = _settingsProvider.Mp3GainPath;

            result.ShouldBe(mp3GainPath);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Mp3GainPath_WhenCalledButSettingsRetrieverReturnsNull_ThenReturnsDefaultExeLocation(string value)
        {
            _settingsRetrieverMock
                .Setup(m => m.Mp3GainPath)
                .Returns(value);

            var result = _settingsProvider.Mp3GainPath;

            result.ShouldBe($"{GetFolderPath(SpecialFolder.ProgramFilesX86)}\\MP3Gain\\mp3gain.exe");
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void IsOutputDirectoryValid_WhenCalled_ReturnsStorageWrapperResult(bool returnValue)
        {
            _settingsRetrieverMock
                .Setup(m => m.OutputPath)
                .Returns(@"C:\Temp");

            _storageWrapperMock
                .Setup(m => m.DirectoryExists(_settingsProvider.OutputDirectory))
                .Returns(returnValue);

            var result = _settingsProvider.IsOutputDirectoryValid();

            result.ShouldBe(returnValue);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void IsMp3GainPathValid_WhenCalled_ReturnsStorageWrapperResult(bool returnValue)
        {
            _settingsRetrieverMock
                .Setup(m => m.Mp3GainPath)
                .Returns(@"C:\Temp\mp3gain.exe");

            _storageWrapperMock
                .Setup(m => m.FileExists(_settingsProvider.Mp3GainPath))
                .Returns(returnValue);

            var result = _settingsProvider.IsMp3GainPathValid();

            result.ShouldBe(returnValue);
        }

        [TestMethod]
        public void ThemeBaseColor_WhenCalled_GetsThemeBaseColorFromSettingsRetriever()
        {
            const string expectedThemeBaseColor = "Light";

            _settingsRetrieverMock
                .Setup(m => m.ThemeBaseColor)
                .Returns(expectedThemeBaseColor);

            var result = _settingsProvider.ThemeBaseColor;

            result.ShouldBe(expectedThemeBaseColor);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ThemeBaseColor_WhenCalledButSettingsRetrieverReturnsNullOrEmpty_ThenReturnsDefault(string value)
        {
            _settingsRetrieverMock
                .Setup(m => m.ThemeBaseColor)
                .Returns(value);

            var result = _settingsProvider.ThemeBaseColor;

            result.ShouldBe("Dark");
        }

        [TestMethod]
        public void ThemeColorScheme_WhenCalled_GetsThemeBaseColorFromSettingsRetriever()
        {
            const string expectedThemeColorScheme = "Cyan";

            _settingsRetrieverMock
                .Setup(m => m.ThemeColorScheme)
                .Returns(expectedThemeColorScheme);

            var result = _settingsProvider.ThemeColorScheme;

            result.ShouldBe(expectedThemeColorScheme);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ThemeColorScheme_WhenCalledButSettingsRetrieverReturnsNullOrEmpty_ThenReturnsDefault(string value)
        {
            _settingsRetrieverMock
                .Setup(m => m.ThemeColorScheme)
                .Returns(value);

            var result = _settingsProvider.ThemeColorScheme;

            result.ShouldBe(ThemeResources.Olive);
        }
    }
}