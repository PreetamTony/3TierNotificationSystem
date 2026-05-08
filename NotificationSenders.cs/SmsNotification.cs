using System;
using NotificationSystem.Interfaces;
using NotificationApp.Models;

namespace NotificationSystem.NotificationSenders
{
    public class SmsNotificationSender : INotificationSender
    {
        public string NotificationType => "SMS";

        public bool CanSendTo(User user)
        {
            return user != null && user.HasValidPhone;
        }

        public void Send(User user, Notification notification)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));
            if (string.IsNullOrWhiteSpace(user.Phone))
                throw new ArgumentException("User phone is required for SMS notification", nameof(user));

            Console.WriteLine("\n=============== SMS NOTIFICATION ===============");
            Console.WriteLine($"To: +91 {user.Phone}");
            Console.WriteLine($"Message: {notification.Message}");
            Console.WriteLine($"Sent at: {notification.SentDate:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine("================================================");
            Console.WriteLine("SMS sent successfully!");
        }
    }
}