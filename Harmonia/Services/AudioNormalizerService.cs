using System.Diagnostics;
using System.IO;
using Harmonia.Services.Interfaces;
using Harmonia.Settings;
using Harmonia.Settings.Interfaces;
using Harmonia.Wrappers.Interfaces;

namespace Harmonia.Services
{
    public class AudioNormalizerService(
        ISettingsManager settingsManager,
        IProcessWrapper processWrapper,
        IStorageWrapper storageWrapper) : IAudioNormalizerService
    {
        private readonly UserSettings _userSettings = settingsManager.LoadSettings();
        private readonly IProcessWrapper _processWrapper = processWrapper;
        private readonly IStorageWrapper _storageWrapper = storageWrapper;

        public void NormalizeAudio(string mp3Path)
        {
            if (!_storageWrapper.FileExists(_userSettings.Mp3GainPath))
            {
                return;
            }

            if (!_storageWrapper.FileExists(mp3Path))
            {
                throw new FileNotFoundException($"MP3 was not found at the specified path {mp3Path}. Audio normalization failed.");
            }

            var processStartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                FileName = _userSettings.Mp3GainPath,
                Arguments = $"/r /k /c \"{mp3Path}\""
            };

            _processWrapper.StartWaitForExitWithTimeoutKill(processStartInfo, timeoutSeconds: 600);
        }
    }
}