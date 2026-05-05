using System;
using System.Linq;
using NotificationApp.Models;
using NotificationSystem.Interfaces;
using NotificationSystem.Services;

namespace NotificationSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            NotificationCRUDService crudService = new NotificationCRUDService();
            NotificationService notificationService = new NotificationService();

            Console.WriteLine("\n****** Notification System ******");

            while (true)
            {
                Console.WriteLine("\n=== Main Menu ===");
                Console.WriteLine("1. Create Notification");
                Console.WriteLine("2. View All Notifications");
                Console.WriteLine("3. View Notification by ID");
                Console.WriteLine("4. Update Notification");
                Console.WriteLine("5. Delete Notification");
                Console.WriteLine("6. Send Notification");
                Console.WriteLine("7. Clear All Notifications");
                Console.WriteLine("8. Exit");
                Console.Write("Enter your choice: ");

                string choiceStr = Console.ReadLine() ?? "";
                if (!int.TryParse(choiceStr, out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1: // Create
                            Console.Write("Enter notification message: ");
                            string message = Console.ReadLine() ?? "";
                            crudService.Create(message);
                            break;

                        case 2: // Read All
                            crudService.ReadAll();
                            break;

                        case 3: // Read by ID
                            Console.Write("Enter notification ID: ");
                            if (int.TryParse((Console.ReadLine() ?? ""), out int readId))
                                crudService.Read(readId);
                            else
                                Console.WriteLine("Invalid ID.");
                            break;

                        case 4: // Update
                            Console.Write("Enter notification ID to update: ");
                            if (int.TryParse((Console.ReadLine() ?? ""), out int updateId))
                            {
                                Console.Write("Enter new message: ");
                                string newMessage = Console.ReadLine() ?? "";
                                crudService.Update(updateId, newMessage);
                            }
                            else
                                Console.WriteLine("Invalid ID.");
                            break;

                        case 5: // Delete
                            Console.Write("Enter notification ID to delete: ");
                            if (int.TryParse((Console.ReadLine() ?? ""), out int deleteId))
                                crudService.Delete(deleteId);
                            else
                                Console.WriteLine("Invalid ID.");
                            break;

                        case 6: // Send Notification
                            SendNotificationMenu(notificationService, crudService);
                            break;

                        case 7: // Clear All
                            Console.Write("Are you sure you want to clear all notifications? (y/N): ");
                            string confirm = (Console.ReadLine() ?? "").ToLower();
                            if (confirm == "y" || confirm == "yes")
                                crudService.ClearAll();
                            break;

                        case 8: // Exit
                            Console.WriteLine("Goodbye!");
                            return;

                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        static void SendNotificationMenu(NotificationService notificationService, NotificationCRUDService crudService)
        {
            Console.WriteLine("\n=== Send Notification ===");

            // Get user details
            Console.Write("Enter Your Name: ");
            string name = Console.ReadLine() ?? "";
            Console.Write("Enter Your Email: ");
            string email = Console.ReadLine() ?? "";
            Console.Write("Enter Your Phone: ");
            string phone = Console.ReadLine() ?? "";
            User user = new User(name, email, phone);

            // Show existing notifications
            var notifications = crudService.ReadAll();
            if (notifications.Count == 0)
            {
                Console.WriteLine("No notifications available. Please create one first.");
                return;
            }

            Console.Write("Enter notification ID to send (or 0 to create new): ");
            if (!int.TryParse((Console.ReadLine() ?? ""), out int notificationId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            Notification notification;
            if (notificationId == 0)
            {
                Console.Write("Enter new notification message: ");
                string message = Console.ReadLine() ?? "";
                notification = crudService.Create(message);
            }
            else
            {
                notification = notifications.FirstOrDefault(n => n.Id == notificationId) ?? null!;
                if (notification == null)
                {
                    Console.WriteLine($"Notification with ID {notificationId} not found.");
                    return;
                }
            }

            Console.WriteLine("Choose Notification Method: 1. Email 2. SMS");
            Console.Write("Enter Your Choice: ");
            if (!int.TryParse((Console.ReadLine() ?? ""), out int choice))
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            INotification? notificationType = null;
            switch (choice)
            {
                case 1:
                    notificationType = new EmailNotification();
                    break;
                case 2:
                    notificationType = new SmsNotification();
                    break;
                default:
                    Console.WriteLine("Invalid Choice");
                    return;
            }

            notificationService.SendNotification(notificationType, user, notification);
        }
    }
}