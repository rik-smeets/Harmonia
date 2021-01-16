using System.ComponentModel;
using System.Configuration;
using Harmonia.Settings.Interfaces;

namespace Harmonia.Settings
{
    public class SettingsRetriever : ApplicationSettingsBase, ISettingsRetriever
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

        public override void Upgrade() => base.Upgrade();
    }
}