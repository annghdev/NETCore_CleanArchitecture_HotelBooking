using HotelBooking.Application.Features.Auth.Common;
using HotelBooking.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HotelBooking.Infrastructure.Auth;

public class TokenGenerator : ITokenGenerator
{
    private readonly IConfiguration _configuration;
    private readonly string _jwtSecret;
    private readonly string _issuer;
    private readonly string _audience;

    public TokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
        _jwtSecret = _configuration["Auth:JwtSecret"] ?? "HotelBooking@JWT@Secret@2024!@VeryLongSecretKey@ForSecurity";
        _issuer = _configuration["Auth:Issuer"] ?? "HotelBooking.API";
        _audience = _configuration["Auth:Audience"] ?? "HotelBooking.Client";
    }

    public Task<string> GenerateAccessTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("phone_number", user.PhoneNumber ?? string.Empty),
            new("full_name", user.FullName ?? string.Empty),
            new("confirmed", user.IsConfirmed.ToString()),
            new("account_origin", user.AccountOrigin.ToString() ?? "System")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60), // 1 hour
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Task.FromResult(tokenString);
    }

    public Task<string> GenerateRefreshTokenAsync(User user)
    {
        // Tạo refresh token đơn giản bằng random bytes
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        var refreshToken = Convert.ToBase64String(randomBytes);
        return Task.FromResult(refreshToken);
    }
}
