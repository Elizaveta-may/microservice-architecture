namespace MedVisit.PaymentService.Models
{
    public class DepositResult
    {
        public bool Success { get; set; }
        public decimal Balance { get; set; }
        public string Message { get; set; }
    }

}
