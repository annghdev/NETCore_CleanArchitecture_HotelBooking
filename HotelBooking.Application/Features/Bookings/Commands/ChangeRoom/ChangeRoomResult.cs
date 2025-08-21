namespace HotelBooking.Application.Features.Bookings.Commands.ChangeRoom;

public class ChangeRoomResult
{
    public bool Success { get; set; }
    public DateTimeOffset? ChangedDate { get; set; }
    public string? Message { get; set; }
}
