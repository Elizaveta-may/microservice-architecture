using MedVisit.CatalogService.Entities;

namespace MedVisit.CatalogService.Models
{
    public class MedServiceDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public List<MedicalWorkerDto> MedicalWorkers { get; set; }
        public int OrganizationId { get; set; }
        public bool IsActive { get; set; }
    }
}
