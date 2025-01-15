using System.ComponentModel.DataAnnotations;

namespace MedVisit.BookingService.Models
{
    public class CreateOrderRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Order amount must be greater than 0.")]
        public decimal OrderAmount { get; set; }
        public string ServiceName { get; set; }
    }

}
