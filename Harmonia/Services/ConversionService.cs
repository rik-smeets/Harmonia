using System.Diagnostics;
using System.IO;
using Harmonia.Services.Interfaces;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace Harmonia.Services
{
    public class ConversionService : IConversionService
    {
        public async Task<string> ConvertMp4ToMp3AndGetMp3Path(string mp4Path)
        {
            if (!File.Exists(mp4Path))
            {
                throw new FileNotFoundException($"MP4 was not found at the specified path {mp4Path}. Audio conversion failed.");
            }

            var executablePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, executablePath);
            FFmpeg.SetExecutablesPath(executablePath);

            var mp3Path = mp4Path.Replace(".mp4", ".mp3");
            await (await FFmpeg.Conversions.FromSnippet.Convert(mp4Path, mp3Path))
                .SetOutputFormat(Format.mp3)
                .SetOverwriteOutput(true)
                .AddParameter("-qscale:a 0")
                .Start();

            File.Delete(mp4Path);

            return mp3Path;
        }
    }
}