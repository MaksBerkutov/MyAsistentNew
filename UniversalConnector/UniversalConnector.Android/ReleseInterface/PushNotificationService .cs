using Android.App;
using Android.Content;
using Android.Support.V4.App;
using AndroidX.Core.App;
using UniversalConnector.Droid;
using UniversalConnector.Interface;

[assembly: Xamarin.Forms.Dependency(typeof(UniversalConnector.Droid.PushNotificationService))]
namespace UniversalConnector.Droid
{
    public class PushNotificationService : IPushNotificationService
    {
        public void HandleNotification(string message)
        {
            var intent = new Intent(Application.Context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("message", message);

            PendingIntent pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.OneShot | PendingIntentFlags.Immutable);

            var notificationBuilder = new NotificationCompat.Builder(Application.Context, "channel_1")
                .SetSmallIcon(Resource.Drawable.icon_feed)
                .SetContentTitle("Push Notification")
                .SetContentText(message)
                .SetAutoCancel(false)
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}
