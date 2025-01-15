namespace MedVisit.BookingService.Models
{
    public class NotificationEvent
    {
        public string EventType { get; set; } 
        public int UserId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
