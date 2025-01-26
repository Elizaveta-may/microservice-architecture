using MedVisit.PaymentService.Enums;

namespace MedVisit.PaymentService.Models
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public TransactionStatus Status { get; set; }
    }
}
