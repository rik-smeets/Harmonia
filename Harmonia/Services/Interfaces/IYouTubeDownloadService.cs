using YoutubeExplode.Videos;

namespace Harmonia.Services.Interfaces
{
    public interface IYouTubeDownloadService
    {
        Task<string> DownloadMp4AndGetPath(Video video, string requestedFileName);

        Task<Video> GetVideo(string youTubeId);
    }
}