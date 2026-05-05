using System;
using System.Collections.Generic;
using System.Linq;
using NotificationApp.Models;

namespace NotificationSystem.Services{
    public class NotificationCRUDService{
        private static List<Notification> notifications = new List<Notification>();
        private static int nextId = 1;
        
        public Notification Create(string message){
            var notification = new Notification(message);
            notification.Id = nextId++;
            notifications.Add(notification);
            Console.WriteLine($"Notification created with ID: {notification.Id}");
            return notification;
        }
        
        public Notification? Read(int id){
            var notification = notifications.FirstOrDefault(n => n.Id == id);
            if(notification != null){
                Console.WriteLine($"Notification ID : {notification.Id}");
                Console.WriteLine($"Notification Message : {notification.message}");
                Console.WriteLine($"Notification Sent Date : {notification.Sentdate}");
            }
            else{
                Console.WriteLine($"Notification with ID {id} not found");
            }
            return notification;
        }
        
        public List<Notification> ReadAll(){
            if (notifications.Count == 0){
                Console.WriteLine("No notifications found");
                return new List<Notification>();
            }
            
            Console.WriteLine("All notifications:");
            foreach(var notification in notifications){
                Console.WriteLine($"Notification ID : {notification.Id}");
                Console.WriteLine($"Notification Message : {notification.message}");
                Console.WriteLine($"Notification Sent Date : {notification.Sentdate}");
            }
            return notifications;
        }
        
        public bool Update(int id, string newMessage){
            var notification = notifications.FirstOrDefault(n=> n.Id == id);
            if(notification != null){
                notification.message = newMessage;
                Console.WriteLine($"Notification with ID {id} updated Successfully");
                return true;
            }
            
            Console.WriteLine($"Notification with ID {id} not found");
            return false;
        }
        
        public bool Delete(int id){
            var notification = notifications.FirstOrDefault(n=> n.Id == id);
            if(notification != null){
                notifications.Remove(notification);
                Console.WriteLine($"Notification with ID {id} deleted successfully");
                return true;
            }
            
            Console.WriteLine($"Notification with ID {id} not found");
            return false;
        }
        
        public void ClearAll(){
            notifications.Clear();
            nextId = 1;
            Console.WriteLine("All notifications cleared");
        }
    }
}