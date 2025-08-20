namespace HotelBooking.Application.Features.Customers;

public class CustomerVM
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string IdentityNo { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;

    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
}
