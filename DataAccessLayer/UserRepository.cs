using System.Collections.Generic;
using System.Linq;
using NotificationApp.Models;

namespace NotificationSystem.DataAccessLayer
{
    public class UserRepository
    {
        private static readonly List<User> _users = new();

        public User Add(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _users.Add(user);
            return user;
        }

        public IReadOnlyList<User> GetAll()
        {
            return _users.AsReadOnly();
        }

        public User? FindByName(string name)
        {
            return _users.FirstOrDefault(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool Delete(string name)
        {
            var user = FindByName(name);
            if (user == null)
                return false;

            return _users.Remove(user);
        }

        public int Count
        {
            get
            {
                return _users.Count;
            }
        }
    }
}
