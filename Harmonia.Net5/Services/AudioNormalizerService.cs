using System.Diagnostics;
using System.IO;
using Harmonia.Services.Interfaces;
using Harmonia.Settings.Interfaces;
using Harmonia.Wrappers.Interfaces;

namespace Harmonia.Services
{
    public class AudioNormalizerService : IAudioNormalizerService
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IProcessWrapper _processWrapper;
        private readonly IStorageWrapper _storageWrapper;

        public AudioNormalizerService(
            ISettingsProvider settingsProvider,
            IProcessWrapper processWrapper,
            IStorageWrapper storageWrapper)
        {
            _settingsProvider = settingsProvider;
            _processWrapper = processWrapper;
            _storageWrapper = storageWrapper;
        }

        public void NormalizeAudio(string mp3Path)
        {
            if (!_settingsProvider.IsMp3GainPathValid())
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
                FileName = _settingsProvider.Mp3GainPath,
                Arguments = $"/r /k /c \"{mp3Path}\""
            };

            _processWrapper.StartWaitForExitWithTimeoutKill(processStartInfo, timeoutSeconds: 600);
        }
    }
}