using HotelBooking.Application.Features.Auth.Login;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Auth;

public interface IFindUserStrategy
{
    Task<User> FindAsync(string credential, OpenPlatform openPlatform);
}
