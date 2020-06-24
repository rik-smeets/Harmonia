namespace Harmonia.Wrappers.Interfaces
{
    public interface IStorageWrapper
    {
        bool DirectoryExists(string directory);

        bool FileExists(string filePath);
    }
}