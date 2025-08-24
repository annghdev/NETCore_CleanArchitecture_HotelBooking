using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Repositories;

public interface IUserTokenRepository
{
    Task<User?> FindUserAsync(string token);
    Task AddOrUpdateTokenAsync(Guid userId, string token, TokenType tokenType);
    Task RemoveTokenAsync(string token);
}
