using AutoMapper;
using HotelBooking.Application.Features.Auth.Common;
using HotelBooking.Application.Features.Users;
using HotelBooking.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly Dictionary<AccountOrigin, IFindUserStrategy> _findUserStrategies;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IEnumerable<IFindUserStrategy> findUserStrategies)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        
        // Map strategies based on naming convention
        _findUserStrategies = findUserStrategies.ToDictionary(
            strategy => GetAccountOriginFromStrategy(strategy),
            strategy => strategy);
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Kiểm tra strategy có hỗ trợ AccountOrigin này không
            if (!_findUserStrategies.TryGetValue(request.AccountOrigin, out var strategy))
            {
                return new AuthResponse(
                    Success: false,
                    AccessToken: null,
                    Message: $"Authentication method '{request.AccountOrigin}' is not supported");
            }

            // Tìm user bằng strategy
            var user = await strategy.FindAsync(request.Credential ?? string.Empty, request.Platform);

            // Verify password (trừ Google OAuth)
            if (request.AccountOrigin != AccountOrigin.Google)
            {
                var isPasswordValid = await VerifyPasswordAsync(user, request.Credential ?? string.Empty);
                if (!isPasswordValid)
                {
                    // Tăng login failed count
                    await IncrementLoginFailedCountAsync(user);
                    
                    return new AuthResponse(
                        Success: false,
                        AccessToken: null,
                        Message: "Invalid credentials");
                }
            }

            // Reset login failed count khi login thành công
            if (user.LoginFailedCount > 0)
            {
                user.LoginFailedCount = 0;
                await _userRepository.UpdateAsync(user);
            }

            // Generate tokens
            var accessToken = await _tokenGenerator.GenerateAccessTokenAsync(user);
            var refreshToken = await _tokenGenerator.GenerateRefreshTokenAsync(user);

            // Save refresh token to database
            await SaveRefreshTokenAsync(user, refreshToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map user profile
            var userProfile = _mapper.Map<UserProfileVM>(user);

            return new AuthResponse(
                Success: true,
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                Profile: userProfile);
        }
        catch (Exception ex)
        {
            return new AuthResponse(
                Success: false,
                AccessToken: null,
                Message: ex.Message);
        }
    }

    private async Task<bool> VerifyPasswordAsync(User user, string credential)
    {
        // Parse password từ credential
        var (_, _, password) = SplitPassword.Excute(credential);
        
        // Generate hash với cùng method như khi tạo
        var expectedHash = await _passwordHasher.GeneratePasswordHashAsync(user, password);
        
        return user.Password == expectedHash;
    }

    private async Task IncrementLoginFailedCountAsync(User user)
    {
        user.LoginFailedCount++;
        
        // Lock account nếu login failed quá nhiều lần (5 lần)
        if (user.LoginFailedCount >= 5)
        {
            user.UnlockDate = DateTimeOffset.UtcNow.AddMinutes(30); // Lock 30 phút
        }
        
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task SaveRefreshTokenAsync(User user, string refreshToken)
    {
        // Remove old refresh tokens
        var existingTokens = await _userRepository.GetQueryable()
            .Where(u => u.Id == user.Id)
            .SelectMany(u => u.Tokens!)
            .Where(t => t.Type == TokenType.RefreshToken)
            .ToListAsync();

        // TODO: Remove old tokens (cần UserTokenRepository)
        
        // Add new refresh token
        var userToken = new UserTokens
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Value = refreshToken,
            Type = TokenType.RefreshToken,
            CreatedDate = DateTimeOffset.UtcNow,
            ExpiryDate = DateTimeOffset.UtcNow.AddDays(30) // 30 ngày
        };

        // TODO: Add token via UserTokenRepository
        // Hiện tại skip vì chưa có repository cho UserTokens
    }

    private static AccountOrigin GetAccountOriginFromStrategy(IFindUserStrategy strategy)
    {
        return strategy.GetType().Name switch
        {
            "FindUserByEmail" => AccountOrigin.System,
            "FindUserByUserName" => AccountOrigin.System,
            "FindUserByPhoneNumber" => AccountOrigin.System,
            "FindOrCreateUserByGoogleOAuth" => AccountOrigin.Google,
            _ => AccountOrigin.System
        };
    }
}
