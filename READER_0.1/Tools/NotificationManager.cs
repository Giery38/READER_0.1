using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace READER_0._1.Tools
{
    public static class NotificationManager
    {
        public static EventHandler ShowNotificationEvent { get;  set; }

        public static Brush ErrorColor { get; private set; } = Brushes.Red;
        public static Brush WarningColor { get; private set; } = Brushes.Green;

        public static void ShowNotification(Notification notification)
        {
            ShowNotificationEvent?.Invoke(notification, new EventArgs());            
        }        
    }
}
