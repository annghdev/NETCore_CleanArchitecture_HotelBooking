using HotelBooking.Domain.Abstractions;
using HotelBooking.Domain.ValueObjects;

namespace HotelBooking.Domain.Entities;

public class Role : AuditableEntity<Guid>, IAggregateRoot
{
    public string Name { get; set; } = default;
    public virtual ICollection<RolePermission>? Permissions { get; set; }
}
