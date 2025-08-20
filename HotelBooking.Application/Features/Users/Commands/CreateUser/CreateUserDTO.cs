using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Users.Commands.CreateUser;

public class CreateUserDTO
{
    public string UserName { get; set; } = default!;
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public AccountOrigin? AccountOrigin { get; set; }
}
