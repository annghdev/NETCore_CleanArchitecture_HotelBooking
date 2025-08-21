using HotelBooking.Domain.Entities.Bases;

namespace HotelBooking.Domain.Entities;

public class Role : AuditableEntity<Guid>, IAggregateRoot
{
    public string Name { get; set; } = default!;
    public virtual ICollection<RolePermission>? Permissions { get; set; }
}
