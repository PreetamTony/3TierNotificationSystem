using System;
using NotificationApp.Models;
using NotificationApp.Models;

namespace NotificationApp.Business.NotificationSenders
{
    public class EmailNotificationSender : INotificationSender
    {
        public string NotificationType => "Email";

        public bool CanSendTo(User user)
        {
            return user != null && user.HasValidEmail;
        }

        public void Send(User user, Notification notification)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("User email is required for email notification", nameof(user));
            Console.WriteLine("\n=============== EMAIL NOTIFICATION ===============");
            Console.WriteLine($"To: {user.Email}");
            Console.WriteLine($"Message: {notification.Message}");
            Console.WriteLine($"Sent at: {notification.SentDate:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine("================================================");
            Console.WriteLine("Email sent successfully!");
        }
    }
}