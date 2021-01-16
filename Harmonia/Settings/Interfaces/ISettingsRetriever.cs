namespace Harmonia.Settings.Interfaces
{
    public interface ISettingsRetriever
    {
        bool IsUpgradeRequired { get; set; }
        string Mp3GainPath { get; set; }
        string OutputPath { get; set; }
        string ThemeBaseColor { get; set; }
        string ThemeColorScheme { get; set; }

        void Upgrade();
    }
}