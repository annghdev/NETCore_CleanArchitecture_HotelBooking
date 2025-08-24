using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Repositories;

public interface IUserTokenRepository
{
    Task<User?> FindUserAsync(string token);
    Task AddOrUpdateToken(Guid userId, string token, TokenType tokenType);
    Task RemoveToken(string token);
}
