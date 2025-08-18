using HotelBooking.Domain.Abstractions;

namespace HotelBooking.Domain.Entities;

public class Invoice : AuditableEntity<Guid>, IAggregateRoot
{
}
