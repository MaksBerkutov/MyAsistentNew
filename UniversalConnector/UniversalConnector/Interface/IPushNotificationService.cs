using System;
using System.Collections.Generic;
using System.Text;

namespace UniversalConnector.Interface
{
    public interface IPushNotificationService
    {
        void HandleNotification(string message);
    }
}
