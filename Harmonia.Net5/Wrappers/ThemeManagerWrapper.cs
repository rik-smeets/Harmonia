using System.Windows;
using ControlzEx.Theming;
using Harmonia.Wrappers.Interfaces;

namespace Harmonia.Wrappers
{
    public class ThemeManagerWrapper : IThemeManagerWrapper
    {
        public void ChangeThemeBaseColor(string baseColor)
            => ThemeManager.Current.ChangeThemeBaseColor(Application.Current, baseColor);

        public void ChangeThemeColorScheme(string colorScheme)
            => ThemeManager.Current.ChangeThemeColorScheme(Application.Current, colorScheme);
    }
}