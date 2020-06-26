using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Harmonia.Services.Interfaces;
using Harmonia.Settings.Interfaces;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Harmonia.Services
{
    public class YouTubeDownloadService : IYouTubeDownloadService
    {
        private readonly YoutubeClient _youTubeClient = new YoutubeClient();
        private readonly ISettingsProvider _settingsProvider;

        private static readonly Regex IllegalFileNameRegex =
            new Regex($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))}]");

        public YouTubeDownloadService(ISettingsProvider settingsProvider)
        {
            _youTubeClient = new YoutubeClient();
            _settingsProvider = settingsProvider;
        }

        public async Task<Video> GetVideo(string youTubeId) =>
            await _youTubeClient.Videos.GetAsync(youTubeId);

        public async Task<string> DownloadMp4AndGetPath(Video video, string requestedFileName)
        {
            var streamManifest = await _youTubeClient.Videos.Streams.GetManifestAsync(video.Id);

            var streamInfo = streamManifest
                .GetAudioOnly()
                .Where(s => s.Container == Container.Mp4)
                .WithHighestBitrate();

            var fileExtension = streamInfo.Container.Name;
            var fileName = $"{requestedFileName}.{fileExtension}";

            var formattedFileName = IllegalFileNameRegex.Replace(fileName, string.Empty);

            var outputDirectory = _settingsProvider.OutputDirectory;
            var mp4Path = Path.Combine(outputDirectory, formattedFileName);

            await _youTubeClient.Videos.Streams.DownloadAsync(streamInfo, mp4Path);

            return mp4Path;
        }
    }
}