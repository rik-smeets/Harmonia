namespace Harmonia.Settings.Interfaces
{
    public interface ISettingsProvider
    {
        string OutputDirectory { get; set; }
        string Mp3GainPath { get; set; }
        string ThemeBaseColor { get; set; }
        string ThemeColorScheme { get; set; }

        bool IsMp3GainPathValid();

        bool IsOutputDirectoryValid();
        void UpgradeIfRequired();
    }
}