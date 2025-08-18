using HotelBooking.Domain.Abstractions;
using HotelBooking.Domain.ValueObjects;

namespace HotelBooking.Domain.Entities;

public class User : AuditableEntity<Guid>, IAggregateRoot
{
    public string UserName { get; set; } = default!;
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public AccountOrigin? AccountOrigin { get; set; }
    public virtual ICollection<UserRole>? Roles { get; set; }
    public virtual ICollection<UserPermission>? Permissions { get; set; }
}
public enum AccountOrigin
{
    Register,
    Google,
    Facebook
}