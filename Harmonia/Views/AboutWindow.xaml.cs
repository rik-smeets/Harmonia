using System.Diagnostics;
using System.Reflection;
using System.Windows.Navigation;
using Harmonia.Properties;
using MahApps.Metro.Controls;

namespace Harmonia.Views
{
    public partial class AboutWindow : MetroWindow
    {
        public AboutWindow()
        {
            InitializeComponent();

            var version = Assembly.GetEntryAssembly().GetName().Version;
            var formattedVersion = $"{version.Major}.{version.Minor}.{version.Revision}";

            Title = string.Format(AboutResources.Window_Title, formattedVersion);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}