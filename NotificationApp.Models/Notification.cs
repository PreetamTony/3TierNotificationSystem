using System;

namespace NotificationApp.Models
{
    public class Notification
    {
        private string _message = string.Empty;
        private string _notificationType = string.Empty;
        public int Id { get; set; }

        public string Message
        {
            get => _message;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Message cannot be empty.");
                if (value.Length < 5)
                    throw new ArgumentException("Message must be at least 5 characters.");
                if (value.Length > 1000)
                    throw new ArgumentException("Message cannot exceed 1000 characters.");
                _message = value.Trim();
            }
        }
        public string NotificationType
        {
            get => _notificationType;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Notification type cannot be empty.");
                
                var normalizedType = value.Trim().ToLower();
                if (normalizedType != "email" && normalizedType != "sms")
                    throw new ArgumentException("Notification type must be 'Email' or 'SMS'.");
                
                _notificationType = value.Trim();
            }
        }

        public DateTime SentDate { get; private set; } = DateTime.UtcNow;
        public Notification() { }
        public Notification(string message, string type)
        {
            Message = message;
            NotificationType = type;
            SentDate = DateTime.UtcNow;
        }

    }
}