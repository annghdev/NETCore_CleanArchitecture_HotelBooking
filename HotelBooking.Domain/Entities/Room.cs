using HotelBooking.Domain.Abstractions;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities;

public class Room : AuditableEntity<int>
{
    public string Name { get; set; }
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public int Floor { get; set; }
    public string? MainImageUrl { get; set; }
    public string? ImageUrls { get; set; }
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public RoomStatus Status { get; set; }
    public RoomType Type { get; set; }
    public double HourlyPrice { get; set; }
    public double DailyPrice { get; set; }
}
