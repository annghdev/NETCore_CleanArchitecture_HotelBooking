using HotelBooking.Domain.Abstractions;

namespace HotelBooking.Domain.Entities;

public class Permission : EntityBase<int>
{
    public string Name { get; set; } = default!;
    public PermissionAction ActionType { get; set; }
    public PermissionFeature Feature { get; set; }
}

public enum PermissionAction
{
    Create,
    Update,
    Delete,
    ViewAll,
    ViewOwn,
    DownLoad,
    UpLoad
}
public enum PermissionFeature
{
    Booking,
    Room,
    User,
    Customer,
    Role,
    Pricing
}