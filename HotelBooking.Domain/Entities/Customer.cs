using HotelBooking.Domain.Abstractions;

namespace HotelBooking.Domain.Entities;

public class Customer : AuditableEntity<Guid>, IAggregateRoot
{
    public string FullName { get; set; } = default!;
    public string IdentityNo { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;

    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public string? SessionId { get; set; }
}
