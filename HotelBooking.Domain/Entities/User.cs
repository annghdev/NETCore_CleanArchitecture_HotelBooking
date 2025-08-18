using HotelBooking.Domain.Abstractions;

namespace HotelBooking.Domain.Entities;

public class User : AuditableEntity<Guid>, IAggregateRoot
{
    public DateTimeOffset? DeletedDate { get; set; }
}
