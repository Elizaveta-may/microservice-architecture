using Microsoft.EntityFrameworkCore;
using NotificationService.Models;
using System.Collections.Generic;

namespace NotificationService
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

        public DbSet<Notification> Notifications { get; set; }
    }
}
