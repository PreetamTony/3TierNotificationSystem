namespace NotificationApp.Models
{
    public class User
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";

        public User (string name, string email, string phone)
        {
            Name = name;
            Email = email;
            Phone = phone;
        }
    }
}