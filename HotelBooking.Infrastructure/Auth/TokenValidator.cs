using HotelBooking.Application.Features.Auth;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace HotelBooking.Infrastructure.Auth;

public class TokenValidator : ITokenValidator
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly string _jwtSecret;

    public TokenValidator(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
        _jwtSecret = _configuration["Auth:JwtSecret"] ?? "HotelBooking@JWT@Secret@2024!@VeryLongSecretKey@ForSecurity";
    }

    public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        try
        {
            // Kiểm tra refresh token có tồn tại trong database và chưa expire
            var userTokens = await _userRepository.GetQueryable()
                .Where(u => u.Tokens != null && 
                           u.Tokens.Any(t => t.Value == refreshToken && 
                                            t.Type == TokenType.RefreshToken &&
                                            t.ExpiryDate > DateTimeOffset.UtcNow))
                .ToListAsync();

            return userTokens.Any();
        }
        catch
        {
            return false;
        }
    }
}
