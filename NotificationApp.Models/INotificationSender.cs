using NotificationApp.Models;
namespace NotificationApp.Models
{
    public interface INotificationSender
    {
        string NotificationType { get; }
        bool CanSendTo(User user);
        void Send(User user, Notification notification);
    }
}   