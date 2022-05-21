using System.Windows;
using Harmonia.Settings.Interfaces;
using Harmonia.Views;
using Harmonia.Wrappers.Interfaces;
using Unity;

namespace Harmonia
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var container = Bootstrapper.Register(new UnityContainer().AddExtension(new Diagnostic()));

            var settingsManager = container.Resolve<ISettingsManager>();
            var settings = settingsManager.LoadSettings();

            var themeManagerWrapper = container.Resolve<IThemeManagerWrapper>();
            themeManagerWrapper.ChangeThemeBaseColor(settings.ThemeBaseColor);
            themeManagerWrapper.ChangeThemeColorScheme(settings.ThemeColorScheme);

            Current.MainWindow = container.Resolve<MainWindow>();
            Current.MainWindow.Show();
        }
    }
}
