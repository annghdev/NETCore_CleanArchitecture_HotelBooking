using HotelBooking.Domain.Abstractions;

namespace HotelBooking.Domain.Entities;

public class Role : AuditableEntity<Guid>, IAggregateRoot
{
}
