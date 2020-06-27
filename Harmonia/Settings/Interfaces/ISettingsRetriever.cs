namespace Harmonia.Settings.Interfaces
{
    public interface ISettingsRetriever
    {
        string OutputPath { get; set; }
        string Mp3GainPath { get; set; }
        string ThemeBaseColor { get; set; }
        string ThemeColorScheme { get; set; }
        bool IsUpgradeRequired { get; set; }

        void PerformUpgrade();
    }
}