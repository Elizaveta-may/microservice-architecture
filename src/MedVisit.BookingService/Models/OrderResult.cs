namespace MedVisit.BookingService.Models
{
    public class OrderResult
    {
        public int OrderId { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
