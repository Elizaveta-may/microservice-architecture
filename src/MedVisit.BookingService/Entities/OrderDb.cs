using MedVisit.BookingService.Enums;

namespace MedVisit.BookingService.Entities
{
    public class OrderDb
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ServiceName { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
    }
}
