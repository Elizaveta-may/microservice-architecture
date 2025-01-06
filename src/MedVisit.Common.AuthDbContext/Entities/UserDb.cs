namespace MedVisit.Common.AuthDbContext.Entities;

public class UserDb
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
}
