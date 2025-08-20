using HotelBooking.Domain.Entities.Bases;

namespace HotelBooking.Domain.Entities;

public class User : AuditableEntity<Guid>, IAggregateRoot
{
    public string UserName { get; set; } = default!;
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender? Gender { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsConfirmed { get; set; }
    public int LoginFailedCount { get; set; }
    public DateTimeOffset? UnlockDate { get; set; }
    public AccountOrigin? AccountOrigin { get; set; }
    public virtual ICollection<UserRole>? Roles { get; set; }
    public virtual ICollection<UserPermission>? Permissions { get; set; }
}
public enum AccountOrigin
{
    System,
    Google,
    Facebook
}

public enum Gender
{
    Male,
    Female,
    Other
}