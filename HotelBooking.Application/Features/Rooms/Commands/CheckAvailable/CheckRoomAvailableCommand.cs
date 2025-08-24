using MediatR;

namespace HotelBooking.Application.Features.Rooms.Commands.CheckAvailable;

public record CheckRoomAvailableCommand(int RoomId, DateTimeOffset From, DateTimeOffset? To): IRequest<RoomAvailableResult>;


public class RoomAvailableResult
{
    public bool IsAvailable { get; set; }
    public Guid? BookingId { get; set; }
    public string? BookedBy { get; set; } //CustomerName


    public static RoomAvailableResult Available()
    {
        return new RoomAvailableResult
        {
            IsAvailable = true,
        };
    }

    public static RoomAvailableResult Overlap(Guid BookingId, string? BookedBy)
    {
        return new RoomAvailableResult
        {
            IsAvailable = false,
            BookingId = BookingId,
            BookedBy = BookedBy
        };
    }
}