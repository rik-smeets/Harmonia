using System.Diagnostics;
using System.IO;
using Harmonia.Services;
using Harmonia.Settings;
using Harmonia.Settings.Interfaces;
using Harmonia.Wrappers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace Harmonia.UnitTests.Services
{
    [TestClass]
    public class AudioNormalizerServiceTests
    {
        private const string DefaultMp3Path = @"C:\\Music\\Artist - Title.mp3";

        private MockRepository _mockRepository;
        private readonly UserSettings _userSettings = new UserSettings { Mp3GainPath = "mp3gaine.exe " };
        private Mock<IProcessWrapper> _processWrapperMock;
        private Mock<IStorageWrapper> _storageWrapperMock;
        private AudioNormalizerService _audioNormalizerService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            var settingsManagerMock = _mockRepository.Create<ISettingsManager>();
            settingsManagerMock.Setup(m => m.LoadSettings()).Returns(_userSettings);
            _processWrapperMock = _mockRepository.Create<IProcessWrapper>();
            _storageWrapperMock = _mockRepository.Create<IStorageWrapper>();

            _audioNormalizerService = new AudioNormalizerService(
                settingsManagerMock.Object,
                _processWrapperMock.Object,
                _storageWrapperMock.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockRepository.VerifyAll();
            _mockRepository.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void NormalizeAudio_WhenCalledButMp3GainPathIsInvalid_ThenDoesNothing()
        {
            SetupSettingsProviderMockIsMp3GainPathValid(isValid: false);

            _audioNormalizerService.NormalizeAudio(DefaultMp3Path);
        }

        [TestMethod]
        public void NormalizeAudio_WhenCalledButMp3PathIsInvalid_ThenThrowsException()
        {
            SetupSettingsProviderMockIsMp3GainPathValid(isValid: true);
            SetupStorageWrapperMockFileExists(DefaultMp3Path, fileExists: false);

            Should.Throw<FileNotFoundException>(() => _audioNormalizerService.NormalizeAudio(DefaultMp3Path));
        }

        [TestMethod]
        public void NormalizeAudio_WhenCalledWithValidPathAndMp3GainIsAvailable_ThenStartsProcess()
        {
            SetupSettingsProviderMockIsMp3GainPathValid(isValid: true);
            SetupStorageWrapperMockFileExists(DefaultMp3Path, fileExists: true);

            _processWrapperMock
                .Setup(m => m.StartWaitForExitWithTimeoutKill(
                    It.Is<ProcessStartInfo>(psi =>
                    psi.CreateNoWindow == true &&
                    psi.WindowStyle == ProcessWindowStyle.Hidden &&
                    psi.UseShellExecute == false &&
                    psi.FileName == _userSettings.Mp3GainPath &&
                    psi.Arguments == $"/r /k /c \"{DefaultMp3Path}\""
                ), 600));

            _audioNormalizerService.NormalizeAudio(DefaultMp3Path);
        }

        private void SetupStorageWrapperMockFileExists(string mp3Path, bool fileExists)
            => _storageWrapperMock
                    .Setup(m => m.FileExists(mp3Path))
                    .Returns(fileExists);

        private void SetupSettingsProviderMockIsMp3GainPathValid(bool isValid)
            => _storageWrapperMock
                    .Setup(m => m.FileExists(_userSettings.Mp3GainPath))
                    .Returns(isValid);
    }
}