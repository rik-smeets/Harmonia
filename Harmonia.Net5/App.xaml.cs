using System.Windows;
using Harmonia.Net5.Views;
using Harmonia.Settings.Interfaces;
using Harmonia.Wrappers.Interfaces;
using Unity;

namespace Harmonia.Net5
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var container = Bootstrapper.Register(new UnityContainer().AddExtension(new Diagnostic()));

            var settingsProvider = container.Resolve<ISettingsProvider>();
            settingsProvider.UpgradeIfRequired();

            var themeManagerWrapper = container.Resolve<IThemeManagerWrapper>();
            themeManagerWrapper.ChangeThemeBaseColor(settingsProvider.ThemeBaseColor);
            themeManagerWrapper.ChangeThemeColorScheme(settingsProvider.ThemeColorScheme);

            Current.MainWindow = container.Resolve<MainWindow>();
            Current.MainWindow.Show();
        }
    }
}
