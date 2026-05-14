using System;
using System.Collections.Generic;
using System.Linq;
using NotificationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace NotificationSystem.DataAccessLayer
{
    public class UserRepository
    {
        public User Add(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            using var context = new NotificationDbContext();
            context.Users.Add(user);
            context.SaveChanges();
            return user;
        }

        public List<User> GetAll()
        {
            using var context = new NotificationDbContext();
            return context.Users.OrderBy(u => u.Id).ToList();
        }

        public User? FindByName(string name)
        {
            using var context = new NotificationDbContext();
            return context.Users.FirstOrDefault(u => EF.Functions.ILike(u.Name, name));
        }

        public bool Delete(string name)
        {
            using var context = new NotificationDbContext();
            var user = context.Users.FirstOrDefault(u => EF.Functions.ILike(u.Name, name));
            if (user != null)
            {
                context.Users.Remove(user);
                return context.SaveChanges() > 0;
            }
            return false;
        }

        public int Count
        {
            get
            {
                using var context = new NotificationDbContext();
                return context.Users.Count();
            }
        }
    }
}
