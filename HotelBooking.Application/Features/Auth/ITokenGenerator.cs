using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Auth;

public interface ITokenGenerator
{
    Task<string> GenerateAccessTokenAsync(User user);
    Task<string> GenerateRefreshTokenAsync(User user);
}
