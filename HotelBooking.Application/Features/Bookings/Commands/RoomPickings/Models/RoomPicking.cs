namespace HotelBooking.Application.Features.Bookings.Commands.RoomPickings.Models;

public class RoomPicking
{
    public Guid Id { get; set; }
    public string? SessionId { get; set; }
    public Guid? CustomerId { get; set; }
    public BookingType Type { get; set; }
    public BookingOrigin Origin { get; set; }
    public DateTimeOffset AutoRealeaseDate { get; set; }
    public IEnumerable<int> RoomIds { get; set; } = [];
}
