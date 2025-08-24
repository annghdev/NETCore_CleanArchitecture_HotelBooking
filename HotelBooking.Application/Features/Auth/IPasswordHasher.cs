using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Auth;

public interface IPasswordHasher
{
    Task<string> GeneratePasswordHashAsync(User user, string password);
}
