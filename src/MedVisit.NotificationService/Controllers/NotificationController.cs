using MedVisit.NotificationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.NotificationService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        [HttpGet]
        public async Task<IActionResult> GetUserNotifications([FromServices] NotificationDbContext dbContext)
        {
            var userId = User.FindFirst("user_id")?.Value;

            var notifications = await dbContext.Notifications
                .Where(n => n.UserId == int.Parse(userId))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }
    }
}
