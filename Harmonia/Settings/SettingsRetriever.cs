using Harmonia.Settings.Interfaces;

namespace Harmonia.Settings
{
    public class SettingsRetriever : ISettingsRetriever
    {
        public string OutputPath
        {
            get => Properties.Settings.Default.OutputPath;
            set
            {
                Properties.Settings.Default.OutputPath = value;
                Properties.Settings.Default.Save();
            }
        }

        public string Mp3GainPath
        {
            get => Properties.Settings.Default.Mp3GainPath;
            set
            {
                Properties.Settings.Default.Mp3GainPath = value;
                Properties.Settings.Default.Save();
            }
        }

        public string ThemeBaseColor
        {
            get => Properties.Settings.Default.ThemeBaseColor;
            set
            {
                Properties.Settings.Default.ThemeBaseColor = value;
                Properties.Settings.Default.Save();
            }
        }

        public string ThemeColorScheme
        {
            get => Properties.Settings.Default.ThemeColorScheme;
            set
            {
                Properties.Settings.Default.ThemeColorScheme = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}