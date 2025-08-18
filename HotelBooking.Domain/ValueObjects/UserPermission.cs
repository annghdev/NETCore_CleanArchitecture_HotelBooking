using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.ValueObjects;

public class UserPermission
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public int PermissionId { get; set; }
    public Permission? Permission { get; set; }
}
