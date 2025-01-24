using MedVisit.NotificationService.Entities;
using MedVisit.NotificationService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedVisit.NotificationService.Services
{
    public interface INotificationService
    {
        Task SaveNotificationAsync(Notification notification);
    }

    public class NotificationService : INotificationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task SaveNotificationAsync(Notification notification)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

            var entity = new NotificationDb()
            {
                UserId = notification.UserId,
                Subject = notification.Subject,
                Message = notification.Message
            };
            await dbContext.Notifications.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
