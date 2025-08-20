using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Users;

public class UserVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = default!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public AccountOrigin? AccountOrigin { get; set; }
}
