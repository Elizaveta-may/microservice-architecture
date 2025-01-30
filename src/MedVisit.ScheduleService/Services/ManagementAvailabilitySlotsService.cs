using MedVisit.ScheduleService.Entities;
using MedVisit.ScheduleService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.ScheduleService.Services
{
    public interface IManagementAvailabilitySlotsService
    {
        Task CreateAvailabilitySlots(AvailabilitySlotRequest request);

        Task<List<Availability>> GetAllSlotsAsync(int medicalWorkerId, DateTime date);
        Task<bool> FreeSlotAsync(int availabilityId);
        Task<bool> BookSlotAsync(int availabilityId);

    }
    public class ManagementAvailabilitySlotsService : IManagementAvailabilitySlotsService
    {
        private readonly ScheduleDbContext _context;

        public ManagementAvailabilitySlotsService(ScheduleDbContext context)
        {
            _context = context;
        }
        public async Task CreateAvailabilitySlots(AvailabilitySlotRequest request)
        {
            var slots = new List<AvailabilityDb>();
            
            var currentDate = request.StartDate;

            while (currentDate <= request.EndDate)
            {
                slots.Add(new AvailabilityDb
                {
                    MedicalWorkerId = request.MedicalWorkerId,
                    StartTime = currentDate,
                    EndTime = currentDate.AddMinutes(request.TimeSlotInMinutes),
                    IsAvailable = true
                });

                currentDate = currentDate.AddMinutes(request.TimeSlotInMinutes); 
            }

            await _context.Availabilities.AddRangeAsync(slots);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Availability>> GetAllSlotsAsync(int medicalWorkerId, DateTime date)
        {
            var startOfDay = date.Date; 
            var endOfDay = startOfDay.AddDays(1).AddMilliseconds(-1);

            var availableSlotsDb = await _context.Availabilities
                .Where(a => a.MedicalWorkerId == medicalWorkerId
                            && a.StartTime >= startOfDay
                            && a.StartTime <= endOfDay)
                .ToListAsync();

            var availableSlots = availableSlotsDb.Select(aDb => new Availability
            {
                Id = aDb.Id,
                MedicalWorkerId = aDb.MedicalWorkerId,
                StartTime = aDb.StartTime,
                EndTime = aDb.EndTime,
                IsAvailable = aDb.IsAvailable
            }).ToList();
            return availableSlots;
        }

        public async Task<bool> BookSlotAsync(int availabilityId)
        {
            var availability = await _context.Availabilities
                .FirstOrDefaultAsync(a => a.Id == availabilityId && a.IsAvailable);

            if (availability == null)
            {
                return false;
            }

            availability.IsAvailable = false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> FreeSlotAsync(int availabilityId)
        {
            var availability = await _context.Availabilities
                .FirstOrDefaultAsync(a => a.Id == availabilityId && !a.IsAvailable);

            if (availability == null)
            {
                return false;
            }

            availability.IsAvailable = true;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
