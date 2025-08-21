using HotelBooking.Domain.Entities.Bases;

namespace HotelBooking.Domain.Entities;

public class BookingRoom : AuditableEntity<Guid>
{
    public Guid BookingId { get; set; }
    public int RoomId { get; set; }
    public Room? Room { get; set; }
    public string? Notes { get; set; }
    public Guid? ChangedToRoomId { get; set; }
    public BookingRoom? ChangedToRoom { get; set; }
    public DateTimeOffset? ChangedRoomDate { get; set; }
    public double? SubTotal { get; set; }
}
