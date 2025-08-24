using HotelBooking.Application.Features.Auth.Login;

namespace HotelBooking.Application.Features.Auth.Common;

public interface IFindUserStrategy
{
    Task<User> FindAsync(string credential, OpenPlatform openPlatform);
}
