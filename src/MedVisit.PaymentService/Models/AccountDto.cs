namespace MedVisit.PaymentService.Models
{
    public class AccountDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
    }
}
