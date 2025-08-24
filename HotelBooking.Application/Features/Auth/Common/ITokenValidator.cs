namespace HotelBooking.Application.Features.Auth.Common;

public interface ITokenValidator
{
    Task<bool> ValidateRefreshTokenAsync(string refreshToken);
}
