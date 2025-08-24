using HotelBooking.Application.Features.Auth.Common;
using HotelBooking.Application.Features.Auth.Login;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.Repositories;

namespace HotelBooking.Infrastructure.Auth.FindUserStrategies;

public class FindUserByPhoneNumber : IFindUserStrategy
{
    private readonly IUserRepository _userRepository;

    public FindUserByPhoneNumber(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> FindAsync(string credential, OpenPlatform platform)
    {
        // Parse credential: phone|password
        var (schema, phoneNumber, password) = SplitPassword.Excute(credential);
        
        if (schema != "phone")
        {
            throw new ArgumentException("Invalid credential schema for phone authentication");
        }

        // Tìm user theo phone number
        var user = await _userRepository.GetSingleAsync(u => u.PhoneNumber == phoneNumber);
        
        if (user == null)
        {
            throw new NotFoundException("User", phoneNumber);
        }

        // Kiểm tra tài khoản có bị khóa không
        if (user.UnlockDate.HasValue && user.UnlockDate > DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException($"Account is locked until {user.UnlockDate.Value:yyyy-MM-dd HH:mm:ss}");
        }

        return user;
    }
}
