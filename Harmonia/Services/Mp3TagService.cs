using System.IO;
using Harmonia.Services.Interfaces;

namespace Harmonia.Services
{
    public class Mp3TagService : IMp3TagService
    {
        public void SetMp3Tags(string mp3Path, string artist, string title)
        {
            if (!File.Exists(mp3Path))
            {
                throw new FileNotFoundException($"Mp# was not found at the specified path {mp3Path}. Setting of MP3 tags failed.");
            }

            using (TagLib.File file = TagLib.File.Create(mp3Path))
            {
                file.RemoveTags(TagLib.TagTypes.AllTags);
                file.Save();
            }

            using (TagLib.File file = TagLib.File.Create(mp3Path))
            {
                file.Tag.Title = title;
                file.Tag.Performers = new[] { artist };
                file.Tag.AlbumArtists = new[] { artist };
                file.Save();
            }
        }
    }
}