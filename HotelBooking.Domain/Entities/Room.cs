using HotelBooking.Domain.Entities.Bases;

namespace HotelBooking.Domain.Entities;

public class Room : AuditableEntity<int>, IAggregateRoot
{
    public string Name { get; set; } = default!;
    public int Floor { get; set; }
    public string? Description { get; set; }
    public RoomType Type { get; set; }
    public int Capacity { get; set; }
    public string? MainImageUrl { get; set; }
    public string? ImageUrls { get; set; }
    public bool IsActive { get; set; } = true;
}
public enum RoomType
{
    Single,
    Dual,
    Suite
}
