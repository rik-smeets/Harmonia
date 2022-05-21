using System.IO;
using System.Text.RegularExpressions;
using Harmonia.Services.Interfaces;
using Harmonia.Settings;
using Harmonia.Settings.Interfaces;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Harmonia.Services
{
    public class YouTubeDownloadService : IYouTubeDownloadService
    {
        private readonly YoutubeClient _youTubeClient = new();
        private readonly UserSettings _userSettings;
        private static readonly Regex IllegalFileNameRegex =
            new($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))}]");

        public YouTubeDownloadService(ISettingsManager settingsManager)
        {
            _youTubeClient = new YoutubeClient();
            _userSettings = settingsManager.LoadSettings();
        }

        public async Task<Video> GetVideo(string youTubeId) =>
            await _youTubeClient.Videos.GetAsync(youTubeId);

        public async Task<string> DownloadMp4AndGetPath(Video video, string requestedFileName)
        {
            var streamManifest = await _youTubeClient.Videos.Streams.GetManifestAsync(video.Id);

            var streamInfo = streamManifest
                .GetAudioOnlyStreams()
                .Where(s => s.Container == Container.Mp4)
                .GetWithHighestBitrate();

            var fileExtension = streamInfo.Container.Name;
            var fileName = $"{requestedFileName}.{fileExtension}";

            var formattedFileName = IllegalFileNameRegex.Replace(fileName, string.Empty);

            var outputDirectory = _userSettings.OutputDirectory;
            var mp4Path = Path.Combine(outputDirectory, formattedFileName);

            await _youTubeClient.Videos.Streams.DownloadAsync(streamInfo, mp4Path);

            return mp4Path;
        }
    }
}