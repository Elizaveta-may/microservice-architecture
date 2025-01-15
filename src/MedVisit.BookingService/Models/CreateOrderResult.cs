namespace MedVisit.BookingService.Models
{
    public class CreateOrderResult
    {
        public int OrderId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
