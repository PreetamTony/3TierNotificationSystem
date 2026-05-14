using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NotificationApp.Models;

namespace NotificationSystem.DataAccessLayer
{
    public class NotificationRepository
    {
        public Notification Add(Notification notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            using var context = new NotificationDbContext();
            context.Notifications.Add(notification);
            context.SaveChanges();
            return notification;
        }

        public List<NotificationWithUser> GetAll()
        {
            using var context = new NotificationDbContext();
            return context.Notifications
                .Include(n => n.User)
                .OrderByDescending(n => n.SentDate)
                .Select(n => MapToDto(n))
                .ToList();
        }

        public List<NotificationWithUser> GetByType(string type)
        {
            using var context = new NotificationDbContext();
            return context.Notifications
                .Include(n => n.User)
                .Where(n => EF.Functions.ILike(n.NotificationType, type))
                .OrderByDescending(n => n.SentDate)
                .Select(n => MapToDto(n))
                .ToList();
        }

        public Dictionary<string, int> GetNotificationCountsByType()
        {
            using var context = new NotificationDbContext();
            return context.Notifications
                .GroupBy(n => n.NotificationType)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count(),
                    StringComparer.OrdinalIgnoreCase
                );
        }

        public void ClearAll()
        {
            using var context = new NotificationDbContext();
            context.Notifications.ExecuteDelete();
        }

        public int Count
        {
            get
            {
                using var context = new NotificationDbContext();
                return context.Notifications.Count();
            }
        }

        private static NotificationWithUser MapToDto(Notification n) =>
            new NotificationWithUser
            {
                Id = n.Id,
                UserId = n.UserId,
                Message = n.Message,
                NotificationType = n.NotificationType,
                SentDate = n.SentDate,
                UserName = n.User?.Name ?? "Unknown",
                UserEmail = n.User?.Email ?? string.Empty,
                UserPhone = n.User?.Phone ?? string.Empty
            };
    }
}