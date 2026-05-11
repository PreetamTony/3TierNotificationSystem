using System;
using System.Collections.Generic;
using Npgsql;
using NotificationApp.Models;

namespace NotificationSystem.DataAccessLayer
{
    public class UserRepository
    {
        public User Add(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            const string sql = "INSERT INTO users (name, email, phone) VALUES (@name, @email, @phone) RETURNING id;";

            using var conn = new NpgsqlConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("name",  user.Name);
            cmd.Parameters.AddWithValue("email", user.Email ?? string.Empty);
            cmd.Parameters.AddWithValue("phone", user.Phone ?? string.Empty);

            user.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return user;
        }

        public List<User> GetAll()
        {
            const string sql = "SELECT id, name, email, phone FROM users ORDER BY id;";
            var users = new List<User>();

            using var conn = new NpgsqlConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                users.Add(MapUser(reader));

            return users;
        }

        public User? FindByName(string name)
        {
            const string sql = "SELECT id, name, email, phone FROM users WHERE name ILIKE @name LIMIT 1;";

            using var conn = new NpgsqlConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("name", name);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapUser(reader) : null;
        }

        public bool Delete(string name)
        {
            const string sql = "DELETE FROM users WHERE name ILIKE @name;";

            using var conn = new NpgsqlConnection(DatabaseHelper.ConnectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("name", name);

            return cmd.ExecuteNonQuery() > 0;
        }

        public int Count
        {
            get
            {
                const string sql = "SELECT COUNT(*) FROM users;";

                using var conn = new NpgsqlConnection(DatabaseHelper.ConnectionString);
                conn.Open();

                using var cmd = new NpgsqlCommand(sql, conn);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private static User MapUser(NpgsqlDataReader r) => new User
        {
            Id = r.GetInt32(r.GetOrdinal("id")),
            Name = r.GetString(r.GetOrdinal("name")),
            Email = r.GetString(r.GetOrdinal("email")),
            Phone = r.GetString(r.GetOrdinal("phone"))
        };
    }
}
