using HotelBooking.Domain.Abstractions;

namespace HotelBooking.Domain.Entities;

public class Branch : AuditableEntity<int>, IAggregateRoot
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string? Location { get; set; }
}
