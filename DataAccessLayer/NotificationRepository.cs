using System;
using System.Collections.Generic;
using System.Linq;
using NotificationApp.Models;

namespace NotificationSystem.DataAccessLayer
{
    public class NotificationRepository
    {
        private static readonly List<Notification> _notifications = new();
        private static int _nextId = 1;

        public Notification Add(Notification notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            notification.Id = _nextId++;
            _notifications.Add(notification);
            return notification;
        }
        public IReadOnlyList<Notification> GetAll()
        {
            return _notifications.AsReadOnly();
        }

        public IEnumerable<Notification> GetByType(string type)
        {
            return _notifications.Where(n => 
                string.Equals(n.NotificationType, type, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public Dictionary<string, int> GetNotificationCountsByType()
        {
            return _notifications.GroupBy(n => n.NotificationType)
                .ToDictionary(g => g.Key, g => g.Count());
        }


        public void ClearAll()
        {
            _notifications.Clear();
            _nextId = 1;
        }
        public int Count
        {
            get
            {
                return _notifications.Count;
            }
        }
    }
}