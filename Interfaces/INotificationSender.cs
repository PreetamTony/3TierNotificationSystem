using NotificationApp.Models;
namespace NotificationSystem.Interfaces
{
    public interface INotificationSender
    {
        string NotificationType { get; }
        bool CanSendTo(User user);
        void Send(User user, Notification notification);
    }
}   