using System;
namespace NotificationApp.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string message { get; set; } = "";
        public DateTime Sentdate { get; set; } = DateTime.Now;
        public Notification(string message)
        {
            this.message = message;
            Sentdate = DateTime.Now;
        }
    }
}