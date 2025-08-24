using HotelBooking.Application.Features.Auth;
using HotelBooking.Application.Features.Auth.Login;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;

namespace HotelBooking.Infrastructure.Auth.FindUserStrategies;

public class FindUserByUserName : IFindUserStrategy
{
    private readonly IUserRepository _userRepository;

    public FindUserByUserName(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> FindAsync(string credential, OpenPlatform platform)
    {
        // Parse credential: username|password
        var (schema, username, password) = SplitPassword.Excute(credential);
        
        if (schema != "username")
        {
            throw new ArgumentException("Invalid credential schema for username authentication");
        }

        // Tìm user theo username
        var user = await _userRepository.GetSingleAsync(u => u.UserName == username);
        
        if (user == null)
        {
            throw new NotFoundException("User", username);
        }

        // Kiểm tra tài khoản có bị khóa không
        if (user.UnlockDate.HasValue && user.UnlockDate > DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException($"Account is locked until {user.UnlockDate.Value:yyyy-MM-dd HH:mm:ss}");
        }

        return user;
    }
}
