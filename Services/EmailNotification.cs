using System;
using NotificationSystem.Interfaces;
using NotificationApp.Models;
namespace NotificationSystem.Services
{
    public class EmailNotification : INotification
    {
        public void Send(User user, Notification notification)
        {
            Console.WriteLine($"Sending Email to {user.Email} with message: {notification.message} at {notification.Sentdate}");
            Console.WriteLine("Email sent successfully.");
        }
    }
}