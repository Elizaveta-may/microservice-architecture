using MedVisit.BookingService.Enums;

namespace MedVisit.BookingService.Entities
{
    public class OrderDb
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MedServiceId { get; set; }
        public string MedServiceName { get; set; }
        public int MedicalWorkerId { get; set; }
        public string MedicalWorkerFullName { get; set; }
        public int TimeSlotId { get; set; }
        public string TimeSlot { get; set; }

        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
    }
}
