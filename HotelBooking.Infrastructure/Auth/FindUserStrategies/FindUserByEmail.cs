using HotelBooking.Application.Features.Auth.Common;
using HotelBooking.Application.Features.Auth.Login;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Auth.FindUserStrategies;

public class FindUserByEmail : IFindUserStrategy
{
    private readonly IUserRepository _userRepository;

    public FindUserByEmail(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> FindAsync(string credential, OpenPlatform platform)
    {
        // Parse credential: email|password
        var (schema, email, password) = SplitPassword.Excute(credential);
        
        if (schema != "email")
        {
            throw new ArgumentException("Invalid credential schema for email authentication");
        }

        // Tìm user theo email
        var user = await _userRepository.GetSingleAsync(u => u.Email == email);
        
        if (user == null)
        {
            throw new NotFoundException("User", email);
        }

        // Kiểm tra tài khoản có bị khóa không
        if (user.UnlockDate.HasValue && user.UnlockDate > DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException($"Account is locked until {user.UnlockDate.Value:yyyy-MM-dd HH:mm:ss}");
        }

        return user;
    }
}
