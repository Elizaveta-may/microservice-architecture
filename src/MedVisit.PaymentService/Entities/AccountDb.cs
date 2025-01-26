namespace MedVisit.PaymentService.Entities
{
    public class AccountDb
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
