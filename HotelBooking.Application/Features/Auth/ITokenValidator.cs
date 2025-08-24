namespace HotelBooking.Application.Features.Auth;

public interface ITokenValidator
{
    Task<bool> ValidateRefreshTokenAsync(string refreshToken);
}
