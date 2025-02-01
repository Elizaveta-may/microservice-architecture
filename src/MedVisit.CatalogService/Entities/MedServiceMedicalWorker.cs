namespace MedVisit.CatalogService.Entities
{
    public class MedServiceMedicalWorker
    {
        public int MedServiceId { get; set; }
        public MedServiceDb MedService { get; set; }

        public int MedicalWorkerId { get; set; }
        public MedicalWorkerDb MedicalWorker { get; set; }
    }

}
