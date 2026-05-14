using Microsoft.EntityFrameworkCore;
using NotificationApp.Models;

namespace NotificationSystem.DataAccessLayer
{
    public class NotificationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=notification_db;Username=postgres;Password=password");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).HasDefaultValue("");
                entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(10).HasDefaultValue("");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
                entity.Property(e => e.Message).HasColumnName("message").IsRequired().HasMaxLength(1000);
                entity.Property(e => e.NotificationType).HasColumnName("notification_type").IsRequired().HasMaxLength(10);
                entity.Property(e => e.SentDate).HasColumnName("sent_date").IsRequired().HasDefaultValueSql("NOW()");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
