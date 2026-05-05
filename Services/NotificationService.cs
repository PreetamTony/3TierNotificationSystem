using NotificationApp.Models;
using NotificationSystem.Interfaces;
namespace NotificationSystem.Services
{
    public class NotificationService
    {
        public void SendNotification(INotification notificationType, User user, Notification notification)
        {
            notificationType.Send(user, notification);
        }
    }
}