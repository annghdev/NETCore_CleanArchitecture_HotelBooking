namespace HotelBooking.Application.Features.Auth.Common;

public interface IPasswordHasher
{
    Task<string> GeneratePasswordHashAsync(User user, string password);
}
