using System;
using Harmonia.Wrappers.Interfaces;
using Windows.Foundation.Metadata;

namespace Harmonia.Wrappers
{
    public class ApiInformationWrapper : IApiInformationWrapper
    {
        public bool IsToastNotificationManagerAvailable { get; }
            = Environment.OSVersion.Platform == PlatformID.Win32NT &&
            Environment.OSVersion.Version.Major >= 10 &&
            ApiInformation.IsTypePresent("Windows.UI.Notifications.ToastNotificationManager");
    }
}