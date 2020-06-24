using System;
using System.IO;
using System.Reflection;
using Harmonia.Services.Interfaces;
using Windows.UI.Notifications;

namespace Harmonia
{
    public class ToastService : IToastService
    {
        public void ShowToast(string toastMessage)
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText03);

            var stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(toastMessage));

            var imageElements = toastXml.GetElementsByTagName("image");
            var attributes = imageElements[0].Attributes;
            attributes.GetNamedItem("src").NodeValue = Path.GetFullPath("./Assets/harmonia.ico");

            var toast = new ToastNotification(toastXml)
            {
                ExpirationTime = DateTime.Now.AddSeconds(10)
            };

            var assemblyName = Assembly.GetEntryAssembly().GetName().Name;

            ToastNotificationManager
                .CreateToastNotifier(applicationId: assemblyName)
                .Show(toast);
        }
    }
}