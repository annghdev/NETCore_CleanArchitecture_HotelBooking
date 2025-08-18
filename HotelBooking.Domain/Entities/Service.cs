using HotelBooking.Domain.Abstractions;

namespace HotelBooking.Domain.Entities;

public class Service : AuditableEntity<Guid>, IAggregateRoot
{
}
