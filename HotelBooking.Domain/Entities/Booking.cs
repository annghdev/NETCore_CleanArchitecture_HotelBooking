using HotelBooking.Domain.Abstractions;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities;

public class Booking : AuditableEntity<Guid>, IAggregateRoot
{
    public Guid? UserId { get; set; }
    public Guid? CustomerName { get; set; }
    public string? PhoneNumber { get; set; }
    public double OriginalAmount { get; set; }
    public double DiscountAmount { get; set; }
    public double FinalAmount { get; set; }
    public BookingType Type { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
}
