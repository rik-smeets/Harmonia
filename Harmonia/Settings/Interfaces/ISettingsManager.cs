namespace Harmonia.Settings.Interfaces
{
    public interface ISettingsManager
    {
        UserSettings LoadSettings();
        void SaveSettings(UserSettings settings);
    }
}