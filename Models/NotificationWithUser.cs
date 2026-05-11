using System;

namespace NotificationApp.Models
{
    public class NotificationWithUser
    {
        public int    Id               { get; set; }
        public string Message          { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public DateTime SentDate       { get; set; }

        public int    UserId           { get; set; }
        public string UserName         { get; set; } = string.Empty;
        public string UserEmail        { get; set; } = string.Empty;
        public string UserPhone        { get; set; } = string.Empty;
    }
}
