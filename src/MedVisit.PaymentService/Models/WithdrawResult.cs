namespace MedVisit.PaymentService.Models
{
    public class WithdrawResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public decimal Balance { get; set; }
    }

}
