namespace MedVisit.ScheduleService.Entities
{
    public class AvailabilityDb
    {
        public int Id { get; set; } 
        public int MedicalWorkerId { get; set; } 
        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; } 
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; } = DateTime.UtcNow;
    }
}
