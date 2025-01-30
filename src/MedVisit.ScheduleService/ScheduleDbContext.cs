using MedVisit.ScheduleService.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.ScheduleService
{
    public class ScheduleDbContext : DbContext
    {
        public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options) : base(options) { }

        public DbSet<AvailabilityDb> Availabilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AvailabilityDb>().ToTable("Availabilities");
            base.OnModelCreating(modelBuilder);
        }
    }
}
