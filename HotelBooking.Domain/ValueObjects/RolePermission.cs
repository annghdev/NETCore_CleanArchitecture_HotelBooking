using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.ValueObjects;

public class RolePermission
{
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }
    public int PermissionId { get; set; }
    public Permission? Permission { get; set; }
}
