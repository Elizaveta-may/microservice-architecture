using MedVisit.Common.AuthDbContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.Common.AuthDbContext;
public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<UserDb> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserDb>().ToTable("Users");
        base.OnModelCreating(modelBuilder);
    }
}
