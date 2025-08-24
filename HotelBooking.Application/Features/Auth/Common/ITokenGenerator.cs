namespace HotelBooking.Application.Features.Auth.Common;

public interface ITokenGenerator
{
    Task<string> GenerateAccessTokenAsync(User user);
    Task<string> GenerateRefreshTokenAsync(User user);
}
