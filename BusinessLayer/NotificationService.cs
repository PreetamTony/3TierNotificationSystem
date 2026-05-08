using System;
using System.Linq;
using NotificationApp.Models;
using NotificationSystem.Interfaces;
using NotificationSystem.NotificationSenders;
using NotificationSystem.DataAccessLayer;
using NotificationSystem.BusinessLayer;

namespace NotificationSystem.BusinessLayer
{
    public class NotificationService
    {
        private readonly NotificationRepository _repository;

        public NotificationService()
        {
            _repository = new NotificationRepository();
        }
        public void SendNotification(User user, string type, string message)
        {
            ValidateInput(user, type, message);
            ValidateBusinessRules(user, type, message);
            INotificationSender notificationSender = CreateNotificationSender(type);
            Notification notification = CreateNotification(message, notificationSender.NotificationType);
            notificationSender.Send(user, notification);
            _repository.Add(notification);

            Console.WriteLine("\nNotification saved successfully.");
        }

        private void ValidateInput(User user, string type, string message)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Notification type cannot be empty.", nameof(type));

            ValidateMessage(message);
        }

        private void ValidateMessage(string message)
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
            {
                throw new ArgumentException($"User does not have valid contact information for {sender.NotificationType} notification.");
            }

            if (sender.NotificationType == "SMS" && message.Length > 160)
            {
                throw new ArgumentException("SMS message cannot exceed 160 characters.");
            }
        }

        private INotificationSender CreateNotificationSender(string type)
        {
            return type.ToLower() switch
            {
                "email" => new EmailNotificationSender(),
                "sms" => new SmsNotificationSender(),
                _ => throw new ArgumentException($"Unsupported notification type: {type}")
            };
        }

        private Notification CreateNotification(string message, string type)
        {
            return new Notification(message, type);
        }
        public void DisplayAllNotifications()
        {
            var notifications = _repository.GetAll();

            if (!notifications.Any())
            {
                Console.WriteLine("No notifications found.");
                return;
            }

            Console.WriteLine($"\n===== ALL NOTIFICATIONS ({notifications.Count}) =====");

            
            var orderedNotifications = notifications.OrderByDescending(n => n.SentDate);

            foreach (var notification in orderedNotifications)
            {
                DisplayNotification(notification);
            }
            DisplayNotificationStatistics();
        }

        private void DisplayNotification(Notification notification)
        {
            Console.WriteLine("\n--------------------------");
            Console.WriteLine($"ID: {notification.Id:D3}");
            Console.WriteLine($"Type: {notification.NotificationType.ToUpper()}");
            Console.WriteLine($"Message: {notification.Message}");
            Console.WriteLine($"Date: {notification.SentDate:yyyy-MM-dd HH:mm:ss}");
        }


        private void DisplayNotificationStatistics()
        {
            var statistics = _repository.GetNotificationCountsByType();
            
            Console.WriteLine("\n===== NOTIFICATION STATISTICS =====");
            
            foreach (var stat in statistics.OrderByDescending(s => s.Value))
            {
                Console.WriteLine($"{stat.Key.ToUpper()}: {stat.Value}");
            }
        }

        public void DisplayNotificationsByType(string type)
        {
            var notifications = _repository.GetByType(type).ToList();
            
            if (!notifications.Any())
            {
                Console.WriteLine($"No {type} notifications found.");
                return;
            }

            Console.WriteLine($"\n===== {type.ToUpper()} NOTIFICATIONS ({notifications.Count}) =====");
            
            foreach (var notification in notifications.OrderByDescending(n => n.SentDate))
            {
                DisplayNotification(notification);
            }
        }

        public void ClearAllNotifications()
        {
            Console.Write("Are you sure you want to clear all notifications? (Y/N): ");
            var confirmation = Console.ReadLine();
            
            if (string.Equals(confirmation, "Y", StringComparison.OrdinalIgnoreCase))
            {
                _repository.ClearAll();
                Console.WriteLine("All notifications cleared successfully.");
            }
            else
            {
                Console.WriteLine("Operation cancelled.");
            }
        }

        public int GetNotificationCount()
        {
            return _repository.Count;
        }
    }
}