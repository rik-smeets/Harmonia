using System;
using System.IO;
using System.Threading.Tasks;
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

            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);

            var mp3Path = mp4Path.Replace(".mp4", ".mp3");
            var conversion = await FFmpeg.Conversions.FromSnippet.Convert(mp4Path, mp3Path);
            conversion.SetOutputFormat(Format.mp3);
            conversion.SetOverwriteOutput(true);
            conversion.AddParameter("-qscale:a 0");

            await conversion.Start();

            File.Delete(mp4Path);

            return mp3Path;
        }
    }
}