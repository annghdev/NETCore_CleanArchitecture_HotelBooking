using HotelBooking.Domain.Abstractions;

namespace HotelBooking.Domain.Entities;

public class ServiceBill : EntityBase<Guid>, ISoftDeletable
{
    public DateTimeOffset? DeletedDate { get; set; }
}
