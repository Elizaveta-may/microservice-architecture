using NotificationService.Models;

namespace NotificationService.Sevices
{
    public interface INotificationService
    {
        Task SaveNotificationAsync(Notification notification);
    }

    public class NotificationService : INotificationService
    {
        private readonly NotificationDbContext _dbContext;

        public NotificationService(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveNotificationAsync(Notification notification)
        {
            await _dbContext.Notifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
        }
    }
}
