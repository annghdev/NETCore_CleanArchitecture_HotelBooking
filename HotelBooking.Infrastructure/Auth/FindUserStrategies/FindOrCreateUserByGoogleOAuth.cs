using HotelBooking.Application.Features.Auth;
using HotelBooking.Application.Features.Auth.Login;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using HotelBooking.Domain;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace HotelBooking.Infrastructure.Auth.FindUserStrategies;

public class FindOrCreateUserByGoogleOAuth : IFindUserStrategy
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public FindOrCreateUserByGoogleOAuth(IUserRepository userRepository, IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    // if open platform is web, credential is AuthCode
    // else credential is IdToken
    public async Task<User> FindAsync(string credential, OpenPlatform openPlatform)
    {
        // Parse credential: google|userinfo_json
        var (schema, googleToken, _) = SplitPassword.Excute(credential);
        
        if (schema != "google")
        {
            throw new ArgumentException("Invalid credential schema for Google OAuth authentication");
        }

        // TODO: Validate Google token với Google API



        // Hiện tại giả định userinfo_json đã được validate và extract từ token
        GoogleUserInfo userInfo;
        try
        {
            userInfo = JsonSerializer.Deserialize<GoogleUserInfo>(googleToken) 
                ?? throw new ArgumentException("Invalid Google user info");
        }
        catch (JsonException)
        {
            throw new ArgumentException("Invalid Google user info format");
        }

        // Tìm user theo email từ Google
        var existingUser = await _userRepository.GetSingleAsync(u => u.Email == userInfo.Email);

        if (existingUser != null)
        {
            // Cập nhật thông tin từ Google nếu cần
            if (existingUser.AccountOrigin != AccountOrigin.Google)
            {
                existingUser.AccountOrigin = AccountOrigin.Google;
                await _userRepository.UpdateAsync(existingUser);
                await _unitOfWork.SaveChangesAsync();
            }
            return existingUser;
        }

        // Tạo user mới từ Google account
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = userInfo.Email,
            Email = userInfo.Email,
            FullName = userInfo.Name,
            IsConfirmed = true, // Google account đã verified
            AccountOrigin = AccountOrigin.Google,
            CreatedDate = DateTimeOffset.UtcNow,
            AvatarUrl = userInfo.Picture
        };

        await _userRepository.AddAsync(newUser);
        await _unitOfWork.SaveChangesAsync();

        return newUser;
    }
}

// DTO cho Google User Info
public class GoogleUserInfo
{
    public string Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Picture { get; set; }
    public bool? EmailVerified { get; set; }
}
