using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Rooms.DTOs;

public class UpdateRoomDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int Floor { get; set; }
    public string? Description { get; set; }
    public RoomType Type { get; set; }
    public int Capacity { get; set; }
    public string? MainImageUrl { get; set; }
    public string? ImageUrls { get; set; }
    public bool IsActive { get; set; } = true;
}
