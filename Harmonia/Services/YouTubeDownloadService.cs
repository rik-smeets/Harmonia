using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Harmonia.Services.Interfaces;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Harmonia.Services
{
    public class YouTubeDownloadService : IYouTubeDownloadService
    {
        private readonly YoutubeClient _youTubeClient = new YoutubeClient();

        public async Task<Video> GetVideo(string youTubeId) =>
            await _youTubeClient.Videos.GetAsync(youTubeId);

        public async Task<string> DownloadMp4AndGetPath(Video video, string requestedFileName)
        {
            var streamManifest = await _youTubeClient.Videos.Streams.GetManifestAsync(video.Id);

            var streamInfo = streamManifest
                .GetAudioOnly()
                .Where(s => s.Container == Container.Mp4)
                .WithHighestBitrate();

            string fileExtension = streamInfo.Container.Name;
            string fileName = RemoveIllegalFileNameCharacters($"{requestedFileName}.{fileExtension}");
            var mp4Path = Path.Combine(Properties.Settings.Default.OutputPath, fileName);

            await _youTubeClient.Videos.Streams.DownloadAsync(streamInfo, mp4Path);

            return mp4Path;
        }

        private static string RemoveIllegalFileNameCharacters(string path)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }
    }
}