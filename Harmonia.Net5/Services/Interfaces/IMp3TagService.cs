namespace Harmonia.Services.Interfaces
{
    public interface IMp3TagService
    {
        void SetMp3Tags(string mp3Path, string artist, string title);
    }
}