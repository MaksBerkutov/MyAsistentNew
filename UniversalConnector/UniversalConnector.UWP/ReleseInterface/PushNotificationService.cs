using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using UniversalConnector.Interface;

[assembly: Xamarin.Forms.Dependency(typeof(UniversalConnector.UWP.PushNotificationService))]
namespace UniversalConnector.UWP
{
    public class PushNotificationService : IPushNotificationService
    {
        public void HandleNotification(string message)
        {
            ToastContent content = new ToastContentBuilder()
                .AddText("Push Notification")
                .AddText(message)
                .GetToastContent();

            var toast = new ToastNotification(content.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}