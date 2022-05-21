namespace Harmonia.Services.Interfaces
{
    public interface IConversionService
    {
        Task<string> ConvertMp4ToMp3AndGetMp3Path(string mp4Path);
    }
}