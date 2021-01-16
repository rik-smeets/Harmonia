using System.ComponentModel;
using System.Configuration;

namespace Harmonia.Net5.Settings
{
    public class SettingsRetriever : ApplicationSettingsBase
    {
        public static SettingsRetriever Default { get; } = (SettingsRetriever)Synchronized(new SettingsRetriever());

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Save();
            base.OnPropertyChanged(sender, e);
        }

        [UserScopedSetting()]
        [DefaultSettingValue("")]
        public string OutputPath
        {
            get => (string)this[nameof(OutputPath)];
            set => this[nameof(OutputPath)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("")]
        public string Mp3GainPath
        {
            get => (string)this[nameof(Mp3GainPath)];
            set => this[nameof(Mp3GainPath)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("")]
        public string ThemeBaseColor
        {
            get => (string)this[nameof(ThemeBaseColor)];
            set => this[nameof(ThemeBaseColor)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("")]
        public string ThemeColorScheme
        {
            get => (string)this[nameof(ThemeColorScheme)];
            set => this[nameof(ThemeColorScheme)] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("True")]
        public bool IsUpgradeRequired
        {
            get => (bool)this[nameof(IsUpgradeRequired)];
            set => this[nameof(IsUpgradeRequired)] = value;
        }
    }
}
