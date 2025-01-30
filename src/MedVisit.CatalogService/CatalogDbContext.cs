using MedVisit.CatalogService.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

        public DbSet<MedicalWorkerDb> MedicalWorkers { get; set; }
        public DbSet<MedServiceDb> MedServices { get; set; }
        public DbSet<OrganizationDb> Organizations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MedicalWorkerDb>().ToTable("MedicalWorkers");
            modelBuilder.Entity<MedServiceDb>().ToTable("MedServices");
            modelBuilder.Entity<OrganizationDb>().ToTable("Organizations");


            modelBuilder.Entity<MedServiceDb>().HasMany(service => service.MedicalWorkers)
                .WithMany()
                .UsingEntity<MedServiceMedicalWorker>("MedServiceMedicalWorker",
                    right => right.HasOne(rt => rt.MedicalWorker).WithMany().HasForeignKey(t => t.MedicalWorkerId),
                    left => left.HasOne(rt => rt.MedService).WithMany().HasForeignKey(t => t.MedServiceId));

            base.OnModelCreating(modelBuilder);
        }
    }
}
