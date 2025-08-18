using HotelBooking.Domain.Abstractions;

namespace HotelBooking.Domain.Entities;

public class BookingRoom : EntityBase<int>, ISoftDeletable
{
    public DateTimeOffset? DeletedDate { get; set; }
}
