using MedVisit.CatalogService.Entities;

namespace MedVisit.CatalogService.Models
{
    public class MedServiceDto
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public List<int> MedicalWorkerIds { get; set; }
        public int OrganizationId { get; set; }
    }
}
