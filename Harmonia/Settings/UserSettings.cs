using System.IO;
using ControlzEx.Theming;
using Harmonia.Properties;
using static System.Environment;

namespace Harmonia.Settings
{
    public class UserSettings
    {
        public string OutputDirectory { get; set; } = GetFolderPath(SpecialFolder.MyMusic);
        public string Mp3GainPath { get; set; } = Path.Combine(GetFolderPath(SpecialFolder.ProgramFilesX86), "MP3Gain", "mp3gain.exe");
        public string ThemeBaseColor { get; set; } = ThemeManager.BaseColorDarkConst;
        public string ThemeColorScheme { get; set; } = ThemeResources.Olive;
    }
}
