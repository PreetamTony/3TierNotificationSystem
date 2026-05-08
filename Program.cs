using System;
using System.Linq;
using NotificationApp.Models;
using NotificationSystem.BusinessLayer;
using NotificationSystem.Presentation;
using NotificationSystem.DataAccessLayer;

namespace NotificationSystem.Presentation
{
    public class ConsoleMenu
    {
        private readonly NotificationService _notificationService;
        private readonly UserRepository _userRepository;
        private bool _isRunning = true;

        public ConsoleMenu()
        {
            _notificationService = new NotificationService();
            _userRepository = new UserRepository();
        }
        public void Start()
        {
            while (_isRunning)
            {
                try
                {
                    DisplayMainMenu();
                    ProcessMainMenuChoice();
                }
                catch (Exception ex)
                {
                    DisplayError($"An unexpected error occurred: {ex.Message}");
                }
                
                if (_isRunning)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine("\n**********************************");
            Console.WriteLine("           MAIN MENU");
            Console.WriteLine("**********************************");
            Console.WriteLine();
            Console.WriteLine("1. Send Notification");
            Console.WriteLine("2. View All Notifications");
            Console.WriteLine("3. View Notifications by Type");
            Console.WriteLine("4. View Notification Statistics");
            Console.WriteLine("5. Clear All Notifications");
            Console.WriteLine("6. Add User");
            Console.WriteLine("7. View All Users");
            Console.WriteLine("8. Exit");
            Console.WriteLine();
            Console.Write("Enter your choice (1-8): ");
        }
        private void ProcessMainMenuChoice()
        {
            var input = Console.ReadLine();
            
            if (!int.TryParse(input, out int choice) || choice < 1 || choice > 8)
            {
                DisplayError("Invalid choice. Please enter a number between 1 and 8.");
                return;
            }

            switch (choice)
            {
                case 1:
                    SendNotificationMenu();
                    break;
                case 2:
                    ViewAllNotifications();
                    break;
                case 3:
                    ViewNotificationsByTypeMenu();
                    break;
                case 4:
                    ViewNotificationStatistics();
                    break;
                case 5:
                    ClearAllNotifications();
                    break;
                case 6:
                    AddUserMenu();
                    break;
                case 7:
                    ViewAllUsers();
                    break;
                case 8:
                    ExitApplication();
                    break;
            }
        }
        private void SendNotificationMenu()
        {
            Console.WriteLine("\n**********************************");
            Console.WriteLine("        SEND NOTIFICATION");
            Console.WriteLine("**********************************");

            try
            {
                var user = GetUserInput();
                var type = GetNotificationType(user);
                var message = GetMessageInput(type);

                _notificationService.SendNotification(user, type, message);
                DisplaySuccess("Notification sent successfully!");
            }
            catch (ArgumentException ex)
            {
                DisplayError($"Validation Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                DisplayError($"Error sending notification: {ex.Message}");
            }
        }
        private User GetUserInput()
        {
            Console.WriteLine("\n--- User Information ---");
            
            var name = GetValidatedInput("Enter user name: ", "Name cannot be empty.", 
                input => !string.IsNullOrWhiteSpace(input) && input.Length >= 2 && input.Length <= 50);
            
            var email = GetValidatedInput("Enter email address: ", "Invalid email format.", 
                input => string.IsNullOrWhiteSpace(input) || ValidationHelper.IsValidEmail(input));
            
            var phone = GetValidatedInput("Enter phone number (10 digits): ", "Invalid phone format.", 
                input => string.IsNullOrWhiteSpace(input) || ValidationHelper.IsValidPhone(input));

            return new User(name, email, phone);
        }

        private string GetNotificationType(User user)
        {
            var availableTypes = user.GetAvailableNotificationTypes();
            
            if (!availableTypes.Any())
            {
                throw new ArgumentException("User must have at least one valid contact method (email or phone).");
            }

            Console.WriteLine("\n--- Notification Type ---");
            Console.WriteLine("Available types for this user:");
            
            for (int i = 0; i < availableTypes.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {availableTypes[i]}");
            }

            while (true)
            {
                Console.Write($"Select notification type (1-{availableTypes.Length}): ");
                var input = Console.ReadLine();
                
                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= availableTypes.Length)
                {
                    return availableTypes[choice - 1];
                }
                
                DisplayError("Invalid selection. Please try again.");
            }
        }

        private string GetMessageInput(string type)
        {
            Console.WriteLine($"\n--- Message Content ({type}) ---");
            
            var maxLength = type.ToLower() == "sms" ? 160 : 1000;
            var message = GetValidatedInput($"Enter message (5-{maxLength} characters): ", 
                $"Message must be between 5 and {maxLength} characters.", 
                input => input.Length >= 5 && input.Length <= maxLength);

            return message;
        }

        private string GetValidatedInput(string prompt, string errorMessage, Func<string, bool> validator)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine()?.Trim() ?? string.Empty;
                
                if (validator(input))
                    return input;
                
                DisplayError(errorMessage);
            }
        }

        private void ViewAllNotifications()
        {
            _notificationService.DisplayAllNotifications();
        }

        private void ViewNotificationsByTypeMenu()
        {
            Console.WriteLine("\n--- View Notifications by Type ---");
            Console.WriteLine("1. Email Notifications");
            Console.WriteLine("2. SMS Notifications");
            Console.Write("Select type (1-2): ");
            
            var input = Console.ReadLine();
            
            if (input == "1")
                _notificationService.DisplayNotificationsByType("Email");
            else if (input == "2")
                _notificationService.DisplayNotificationsByType("SMS");
            else
                DisplayError("Invalid selection.");
        }

        private void ViewNotificationStatistics()
        {
            var count = _notificationService.GetNotificationCount();
            Console.WriteLine($"\nTotal notifications: {count}");
            
            if (count > 0)
            {
                _notificationService.DisplayAllNotifications();
            }
            else
            {
                Console.WriteLine("No notifications to display.");
            }
        }

        private void ClearAllNotifications()
        {
            _notificationService.ClearAllNotifications();
        }

        private void AddUserMenu()
        {
            Console.WriteLine("\n--------------------");
            Console.WriteLine("        ADD USER");
            Console.WriteLine("--------------------");

            try
            {
                var user = GetUserInput();
                _userRepository.Add(user);
                DisplaySuccess("User added successfully!");
            }
            catch (ArgumentException ex)
            {
                DisplayError($"Validation Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                DisplayError($"Error adding user: {ex.Message}");
            }
        }

        private void ViewAllUsers()
        {
            var users = _userRepository.GetAll();

            if (!users.Any())
            {
                Console.WriteLine("\nNo users found.");
                return;
            }

            Console.WriteLine($"\n===== ALL USERS ({users.Count}) =====");

            foreach (var user in users)
            {
                Console.WriteLine("\n--------------------------");
                Console.WriteLine(user.ToString());
                Console.WriteLine($"Valid Email: {user.HasValidEmail}");
                Console.WriteLine($"Valid Phone: {user.HasValidPhone}");
                Console.WriteLine($"Available Types: {string.Join(", ", user.GetAvailableNotificationTypes())}");
            }
        }

        private void ExitApplication()
        {
            Console.WriteLine("\nThank you for using the Notification System!");
            _isRunning = false;
        }

        private void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nERROR: {message}");
            Console.ResetColor();
        }

        private void DisplaySuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n{message}");
            Console.ResetColor();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var consoleMenu = new ConsoleMenu();
        consoleMenu.Start();
    }
}