using System;
using NotificationSystem.BusinessLayer;

namespace NotificationApp.Models
{
    public class User
    {
        public int Id { get; set; }

        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                if (value.Length < 2 || value.Length > 50)
                    throw new ArgumentException("Name must be between 2 and 50 characters.");
                _name = value.Trim();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !ValidationHelper.IsValidEmail(value))
                    throw new ArgumentException("Invalid email format.");
                _email = value?.Trim() ?? string.Empty;
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !ValidationHelper.IsValidPhone(value))
                    throw new ArgumentException("Invalid phone format. Must be 10 digits.");
                _phone = value?.Trim() ?? string.Empty;
            }
        }

        public User() { }

        public User(string name, string email, string phone)
        {
            Name = name;
            Email = email;
            Phone = phone;
        }

        public override string ToString()
        {
            return $"User: {Name} | Email: {Email} | Phone: {Phone}";
        }

        public bool HasValidEmail => !string.IsNullOrWhiteSpace(Email) && ValidationHelper.IsValidEmail(Email);
        public bool HasValidPhone => !string.IsNullOrWhiteSpace(Phone) && ValidationHelper.IsValidPhone(Phone);

        public string[] GetAvailableNotificationTypes()
        {
            var types = new System.Collections.Generic.List<string>();
            
            if (HasValidEmail)
                types.Add("Email");
            
            if (HasValidPhone)
                types.Add("SMS");
            
            return types.ToArray();
        }
    }
}