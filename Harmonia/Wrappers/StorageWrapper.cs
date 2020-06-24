using System.IO;
using Harmonia.Wrappers.Interfaces;

namespace Harmonia.Wrappers
{
    public class StorageWrapper : IStorageWrapper
    {
        public bool DirectoryExists(string directory) => Directory.Exists(directory);

        public bool FileExists(string filePath) => File.Exists(filePath);
    }
}