using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.Repositories;

public class UserTokenRepository(BookingDbContext dbContext) : IUserTokenRepository
{
    private readonly BookingDbContext _dbContext = dbContext;

    public async Task AddOrUpdateToken(Guid userId, string token, TokenType tokenType)
    {
        var now = DateTimeOffset.UtcNow;
        DateTimeOffset expiryDate;
        if (tokenType == TokenType.AccessToken)
            expiryDate = now.AddMinutes(5);
        else
            expiryDate = now.AddDays(7);

        var existedToken = await _dbContext.UserTokens.SingleOrDefaultAsync(t => t.UserId == userId && t.Type == tokenType);
        if (existedToken == null)
        {
            var newToken = new UserTokens
            {
                Id = Guid.CreateVersion7(),
                UserId = userId,
                Type = tokenType,
                Value = token,
                ExpiryDate = expiryDate
            };
            _dbContext.UserTokens.Add(newToken);
        }
        else
        {
            existedToken.Value = token;
            existedToken.ExpiryDate = expiryDate;
            _dbContext.Update(existedToken);
        }
    }

    public async Task<User?> FindUserAsync(string token)
    {
        var userToken = await _dbContext.UserTokens.SingleOrDefaultAsync(t => t.Value == token);
        return userToken?.User;
    }

    public async Task RemoveToken(string tokenValue)
    {
        var token = await _dbContext.UserTokens.SingleOrDefaultAsync(t => t.Value == tokenValue);
        if (token != null)
            _dbContext.UserTokens.Remove(token); 
    }
}
