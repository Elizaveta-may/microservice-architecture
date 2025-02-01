namespace MedVisit.CatalogService.Entities
{
    public class MedicalWorkerDb
    { 
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public int OrganizationId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; } = DateTime.UtcNow;
    }
}
