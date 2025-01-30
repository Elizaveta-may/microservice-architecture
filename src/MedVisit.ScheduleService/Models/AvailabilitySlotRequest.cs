namespace MedVisit.ScheduleService.Models
{
    public class AvailabilitySlotRequest
    {
        public int MedicalWorkerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TimeSlotInMinutes { get; set; }
    }

}
