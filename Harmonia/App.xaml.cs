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

            var container = Bootstrapper.Register(new UnityContainer());

            var settingsProvider = container.Resolve<ISettingsProvider>();
            var themeManagerWrapper = container.Resolve<IThemeManagerWrapper>();
            themeManagerWrapper.ChangeThemeBaseColor(settingsProvider.ThemeBaseColor);
            themeManagerWrapper.ChangeThemeColorScheme(settingsProvider.ThemeColorScheme);

            Current.MainWindow = container.Resolve<MainWindow>();
            Current.MainWindow.Show();
        }
    }
}