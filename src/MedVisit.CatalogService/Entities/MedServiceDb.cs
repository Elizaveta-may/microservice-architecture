namespace MedVisit.CatalogService.Entities
{
    public class MedServiceDb
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; } = DateTime.UtcNow;
        public int OrganizationId { get; set; }
        public List<MedicalWorkerDb> MedicalWorkers { get; set; } = null!;
    }

}
