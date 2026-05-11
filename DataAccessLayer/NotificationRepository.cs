using System;
using System.Collections.Generic;
using Npgsql;
using NotificationApp.Models;

namespace NotificationSystem.DataAccessLayer
{
    public class NotificationRepository
    {
        private const string JoinSelect = @"
            SELECT  n.id,
                    n.user_id,
                    n.message,
                    n.notification_type,
                    n.sent_date,
                    u.name  AS user_name,
                    u.email AS user_email,
                    u.phone AS user_phone FROM notifications n JOIN users u ON u.id = n.user_id";

        public Notification Add(Notification notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            const string sql = @"
                INSERT INTO notifications (user_id, message, notification_type, sent_date)
                VALUES (@user_id, @message, @type, @sent_date)
                RETURNING id;";

            using var conn = new NpgsqlConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("user_id",   notification.UserId);
            cmd.Parameters.AddWithValue("message",   notification.Message);
            cmd.Parameters.AddWithValue("type",      notification.NotificationType);
            cmd.Parameters.AddWithValue("sent_date", notification.SentDate);

            notification.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return notification;
        }

        public List<NotificationWithUser> GetAll()
        {
            const string sql = JoinSelect + " ORDER BY n.sent_date DESC;";
            return ExecuteJoinQuery(sql, null);
        }

        public List<NotificationWithUser> GetByType(string type)
        {
            const string sql = JoinSelect + " WHERE n.notification_type ILIKE @type" + " ORDER BY n.sent_date DESC;";

            return ExecuteJoinQuery(sql,cmd => cmd.Parameters.AddWithValue("type", type));
        }

        public Dictionary<string, int> GetNotificationCountsByType()
        {
            const string sql = @"
                SELECT notification_type, COUNT(*)::INT AS cnt
                FROM   notifications
                GROUP  BY notification_type;";

            var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            using var conn = new NpgsqlConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            using var cmd  = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var notifType = reader.GetString(0);
                var cnt       = reader.GetInt32(1);
                result[notifType] = cnt;
            }

            return result;
        }

        public void ClearAll()
        {
            const string sql = "DELETE FROM notifications;";

            using var conn = new NpgsqlConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        public int Count
        {
            get
            {
                const string sql = "SELECT COUNT(*) FROM notifications;";

                using var conn = new NpgsqlConnection(DatabaseHelper.ConnectionString);
                conn.Open();

                using var cmd = new NpgsqlCommand(sql, conn);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private static List<NotificationWithUser> ExecuteJoinQuery(
            string sql,
            Action<NpgsqlCommand>? parameterSetup)
        {
            var list = new List<NotificationWithUser>();

            using var conn = new NpgsqlConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            parameterSetup?.Invoke(cmd);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(MapNotificationWithUser(reader));

            return list;
        }

        private static NotificationWithUser MapNotificationWithUser(NpgsqlDataReader r) =>
            new NotificationWithUser
            {
                Id = r.GetInt32(r.GetOrdinal("id")),
                UserId = r.GetInt32(r.GetOrdinal("user_id")),
                Message = r.GetString(r.GetOrdinal("message")),
                NotificationType = r.GetString(r.GetOrdinal("notification_type")),
                SentDate = r.GetDateTime(r.GetOrdinal("sent_date")),
                UserName = r.GetString(r.GetOrdinal("user_name")),
                UserEmail = r.GetString(r.GetOrdinal("user_email")),
                UserPhone = r.GetString(r.GetOrdinal("user_phone"))
            };
    }
}