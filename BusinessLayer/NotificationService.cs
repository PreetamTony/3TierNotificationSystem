using System;
using System.Linq;
using NotificationApp.Models;
using NotificationSystem.Interfaces;
using NotificationSystem.NotificationSenders;
using NotificationSystem.DataAccessLayer;

namespace NotificationSystem.BusinessLayer
{
    public class NotificationService
    {
        private readonly NotificationRepository _notificationRepo;
        private readonly UserRepository _userRepo;

        public NotificationService()
        {
            _notificationRepo =new NotificationRepository();
            _userRepo = new UserRepository();
        }

        public void SendNotification(User user, string type, string message)
        {
            ValidateInput(user, type, message);
            ValidateBusinessRules(user, type, message);

            var existingUser = _userRepo.FindByName(user.Name);
            if (existingUser != null)
            {
                user.Id = existingUser.Id;
            }
            else
            {
                _userRepo.Add(user);
            }

            INotificationSender sender = CreateNotificationSender(type);
            Notification notification  = CreateNotification(message, sender.NotificationType, user.Id);

            sender.Send(user, notification);
            _notificationRepo.Add(notification);

            Console.WriteLine("\nNotification saved to database successfully.");
        }

        public void DisplayAllNotifications()
        {
            var notifications = _notificationRepo.GetAll();

            if (!notifications.Any())
            {
                Console.WriteLine("No notifications found.");
                return;
            }

            Console.WriteLine($"\n===== ALL NOTIFICATIONS ({notifications.Count}) =====");

            foreach (var n in notifications)
                DisplayNotification(n);

            DisplayNotificationStatistics();
        }

        public void DisplayNotificationsByType(string type)
        {
            var notifications = _notificationRepo.GetByType(type);

            if (!notifications.Any())
            {
                Console.WriteLine($"No {type} notifications found.");
                return;
            }

            Console.WriteLine($"\n===== {type.ToUpper()} NOTIFICATIONS ({notifications.Count}) =====");

            foreach (var n in notifications)
                DisplayNotification(n);
        }

        private void DisplayNotificationStatistics()
        {
            var statistics = _notificationRepo.GetNotificationCountsByType();

            Console.WriteLine("\n===== NOTIFICATION STATISTICS =====");

            foreach (var stat in statistics.OrderByDescending(s => s.Value))
                Console.WriteLine($"{stat.Key.ToUpper()}: {stat.Value}");
        }

        public void ClearAllNotifications()
        {
            Console.Write("Are you sure you want to clear all notifications? (Y/N): ");
            var confirmation = Console.ReadLine();

            if (string.Equals(confirmation, "Y", StringComparison.OrdinalIgnoreCase))
            {
                _notificationRepo.ClearAll();
                Console.WriteLine("All notifications cleared successfully.");
            }
            else
            {
                Console.WriteLine("Operation cancelled.");
            }
        }

        public int GetNotificationCount() => _notificationRepo.Count;

        private static void DisplayNotification(NotificationWithUser n)
        {
            Console.WriteLine("\n--------------------------");
            Console.WriteLine($"ID       : {n.Id:D3}");
            Console.WriteLine($"Type     : {n.NotificationType.ToUpper()}");
            Console.WriteLine($"Message  : {n.Message}");
            Console.WriteLine($"Sent At  : {n.SentDate:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Sender   : {n.UserName}");
            Console.WriteLine($"Email    : {(string.IsNullOrWhiteSpace(n.UserEmail) ? "—" : n.UserEmail)}");
            Console.WriteLine($"Phone    : {(string.IsNullOrWhiteSpace(n.UserPhone) ? "—" : n.UserPhone)}");
        }

        private void ValidateInput(User user, string type, string message)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Notification type cannot be empty.", nameof(type));

            ValidateMessage(message);
        }

        private static void ValidateMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.");

            if (message.Length < 5)
                throw new ArgumentException("Message should contain at least 5 characters.");

            if (message.Length > 1000)
                throw new ArgumentException("Message cannot exceed 1000 characters.");
        }

        private void ValidateBusinessRules(User user, string type, string message)
        {
            var sender = CreateNotificationSender(type);

            if (!sender.CanSendTo(user))
                throw new ArgumentException(
                    $"User does not have valid contact information for {sender.NotificationType} notification.");

            if (sender.NotificationType == "SMS" && message.Length > 160)
                throw new ArgumentException("SMS message cannot exceed 160 characters.");
        }

        private static INotificationSender CreateNotificationSender(string type) =>
            type.ToLower() switch
            {
                "email" => new EmailNotificationSender(),
                "sms"   => new SmsNotificationSender(),
                _       => throw new ArgumentException($"Unsupported notification type: {type}")
            };

        private static Notification CreateNotification(string message, string type, int userId) =>
            new Notification(message, type) { UserId = userId };
    }
}