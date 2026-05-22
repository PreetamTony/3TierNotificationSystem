using System.Text.RegularExpressions;
namespace NotificationApp.Models
{
    public static class ValidationHelper{
        public static bool IsValidEmail(string email){
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email,pattern);
        }
        public static bool IsValidPhone(string phone){
            string pattern = @"^[0-9]{10}$";
            return Regex.IsMatch(phone,pattern);
        }
    }
}
