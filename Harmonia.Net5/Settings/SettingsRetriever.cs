using Harmonia.Settings.Interfaces;

namespace Harmonia.Settings
{
    public class SettingsRetriever : ISettingsRetriever
    {
        public string OutputPath
        {
            get => Net5.Properties.Settings.Default.OutputPath;
            set
            {
                Net5.Properties.Settings.Default.OutputPath = value;
                Net5.Properties.Settings.Default.Save();
            }
        }

        public string Mp3GainPath
        {
            get => Net5.Properties.Settings.Default.Mp3GainPath;
            set
            {
                Net5.Properties.Settings.Default.Mp3GainPath = value;
                Net5.Properties.Settings.Default.Save();
            }
        }

        public string ThemeBaseColor
        {
            get => Net5.Properties.Settings.Default.ThemeBaseColor;
            set
            {
                Net5.Properties.Settings.Default.ThemeBaseColor = value;
                Net5.Properties.Settings.Default.Save();
            }
        }

        public string ThemeColorScheme
        {
            get => Net5.Properties.Settings.Default.ThemeColorScheme;
            set
            {
                Net5.Properties.Settings.Default.ThemeColorScheme = value;
                Net5.Properties.Settings.Default.Save();
            }
        }

        public bool IsUpgradeRequired
        {
            get => Net5.Properties.Settings.Default.IsUpgradeRequired;
            set
            {
                Net5.Properties.Settings.Default.IsUpgradeRequired = value;
                Net5.Properties.Settings.Default.Save();
            }
        }

        public void PerformUpgrade() => Net5.Properties.Settings.Default.Upgrade();
    }
}