using MedVisit.PaymentService.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.PaymentService
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

        public DbSet<AccountDb> Accounts { get; set; }
        public DbSet<TransactionDb> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountDb>().ToTable("Accounts");
            modelBuilder.Entity<TransactionDb>().ToTable("Transactions");
            base.OnModelCreating(modelBuilder);
        }
    }
}
