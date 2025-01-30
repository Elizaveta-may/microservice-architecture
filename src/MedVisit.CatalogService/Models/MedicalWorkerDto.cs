using MedVisit.CatalogService.Entities;

namespace MedVisit.CatalogService.Models
{
    public class MedicalWorkerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public int OrganizationId { get; set; }
    }
}
