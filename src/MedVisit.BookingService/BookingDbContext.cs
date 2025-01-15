using MedVisit.BookingService.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.BookingService
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        public DbSet<OrderDb> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDb>().ToTable("Orders");
            base.OnModelCreating(modelBuilder);
        }
    }
}
