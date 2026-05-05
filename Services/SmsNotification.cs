using NotificationSystem.Interfaces;
using NotificationApp.Models;
namespace NotificationSystem.Services
{
    public class SmsNotification : INotification
    {
        public void Send(User user, Notification notification)
        {
            Console.WriteLine($"Sending SMS to +91 {user.Phone} with message: {notification.message} at {notification.Sentdate}");
            Console.WriteLine("SMS sent successfully.");
        }
    }
}