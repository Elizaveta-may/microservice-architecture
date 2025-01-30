using System.ComponentModel.DataAnnotations;

namespace MedVisit.BookingService.Models
{
    public class OrderRequest
    {
        public int MedServiceId { get; set; }
        public string MedServiceName { get; set; }
        public int MedicalWorkerId { get; set; }
        public string MedicalWorkerFullName { get; set; }
        public int TimeSlotId { get; set; }
        public string TimeSlot { get; set;}
        public decimal Amount { get; set; }

    }

}
