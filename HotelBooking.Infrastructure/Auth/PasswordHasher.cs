using HotelBooking.Application.Features.Auth.Common;
using HotelBooking.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace HotelBooking.Infrastructure.Auth;

public class PasswordHasher : IPasswordHasher
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;

    public PasswordHasher(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = _configuration["Auth:PasswordSecret"] ?? "HotelBooking@Default@Secret@2024!";
    }

    public Task<string> GeneratePasswordHashAsync(User user, string password)
    {
        // Tạo salt từ UserId và timestamp để đảm bảo unique
        var salt = $"{user.Id}_{user.CreatedDate:yyyyMMddHHmmss}_{_secretKey}";
        
        // Combine password với salt
        var passwordWithSalt = $"{password}_{salt}";
        
        // Hash using SHA256
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
        
        // Convert to base64 string
        var hashedPassword = Convert.ToBase64String(hashedBytes);
        
        return Task.FromResult(hashedPassword);
    }
}
