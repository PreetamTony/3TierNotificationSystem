using System;
using Npgsql;

namespace NotificationSystem.DataAccessLayer
{
    public static class DatabaseHelper
    {
        public const string ConnectionString =
            "Host=localhost;Port=5432;Database=notification_db;Username=postgres;Password=password";

        private const string AdminConnectionString =
            "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=password";

        public static void EnsureTablesExist()
        {
            CreateDatabaseIfNotExists();

            const string createUsers = @"
                CREATE TABLE IF NOT EXISTS users (
                    id      SERIAL       PRIMARY KEY,
                    name    VARCHAR(50)  NOT NULL,
                    email   VARCHAR(255) NOT NULL DEFAULT '',
                    phone   VARCHAR(10)  NOT NULL DEFAULT ''
                );";

            const string createNotifications = @"
                CREATE TABLE IF NOT EXISTS notifications (
                    id                  SERIAL       PRIMARY KEY,
                    user_id             INT          NOT NULL REFERENCES users(id),
                    message             TEXT         NOT NULL,
                    notification_type   VARCHAR(10)  NOT NULL,
                    sent_date           TIMESTAMP    NOT NULL DEFAULT NOW()
                );";

            try
            {
                using var conn = new NpgsqlConnection(ConnectionString);
                conn.Open();

                using (var cmd = new NpgsqlCommand(createUsers, conn))
                    cmd.ExecuteNonQuery();

                using (var cmd = new NpgsqlCommand(createNotifications, conn))
                    cmd.ExecuteNonQuery();

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("[DB] Tables verified / created successfully.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[DB ERROR] Could not initialise database tables: {ex.Message}");
                Console.ResetColor();
                throw;
            }
        }

        private static void CreateDatabaseIfNotExists()
        {
            try
            {
                using var conn = new NpgsqlConnection(AdminConnectionString);
                conn.Open();

                using var cmdCheck = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = 'notification_db'", conn);
                var exists = cmdCheck.ExecuteScalar() != null;

                if (!exists)
                {
                    using var cmdCreate = new NpgsqlCommand("CREATE DATABASE notification_db", conn);
                    cmdCreate.ExecuteNonQuery();
                    Console.WriteLine("[DB] Database 'notification_db' created.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[DB WARNING] Could not verify/create database: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
