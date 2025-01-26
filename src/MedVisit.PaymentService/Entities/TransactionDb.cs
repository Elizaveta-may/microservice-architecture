namespace MedVisit.PaymentService.Entities
{
    public class TransactionDb
    {
        public int Id { get; set; }
        public required int AccountId { get; set; }
        public required decimal Amount { get; set; }
        public required int TransactionType { get; set; }
        public required int Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; } = DateTime.UtcNow;
    }
}
