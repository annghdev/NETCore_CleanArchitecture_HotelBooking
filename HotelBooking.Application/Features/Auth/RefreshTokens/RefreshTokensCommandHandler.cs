using AutoMapper;
using HotelBooking.Application.Features.Users;
using HotelBooking.Domain;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Features.Auth.RefreshTokens;

public class RefreshTokensCommandHandler : IRequestHandler<RefreshTokensCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITokenValidator _tokenValidator;
    private readonly ITokenGenerator _tokenGenerator;

    public RefreshTokensCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ITokenValidator tokenValidator,
        ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tokenValidator = tokenValidator;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate refresh token
            var isValidToken = await _tokenValidator.ValidateRefreshTokenAsync(request.OldToken);
            if (!isValidToken)
            {
                return new AuthResponse(
                    Success: false,
                    AccessToken: null,
                    Message: "Invalid or expired refresh token");
            }

            // Tìm user từ refresh token
            var user = await _userRepository.GetQueryable()
                .Include(u => u.Tokens)
                .Where(u => u.Tokens != null && 
                           u.Tokens.Any(t => t.Value == request.OldToken &&
                                            t.Type == TokenType.RefreshToken &&
                                            t.ExpiryDate > DateTimeOffset.UtcNow))
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return new AuthResponse(
                    Success: false,
                    AccessToken: null,
                    Message: "User not found for this refresh token");
            }

            // Kiểm tra tài khoản có bị khóa không
            if (user.UnlockDate.HasValue && user.UnlockDate > DateTimeOffset.UtcNow)
            {
                return new AuthResponse(
                    Success: false,
                    AccessToken: null,
                    Message: $"Account is locked until {user.UnlockDate.Value:yyyy-MM-dd HH:mm:ss}");
            }

            // Generate new tokens
            var newAccessToken = await _tokenGenerator.GenerateAccessTokenAsync(user);
            var newRefreshToken = await _tokenGenerator.GenerateRefreshTokenAsync(user);

            // Update refresh token trong database
            await UpdateRefreshTokenAsync(user, request.OldToken, newRefreshToken);
            
            await _unitOfWork.SaveChangesAsync();

            // Map user profile
            var userProfile = _mapper.Map<UserProfileVM>(user);

            return new AuthResponse(
                Success: true,
                AccessToken: newAccessToken,
                RefreshToken: newRefreshToken,
                Profile: userProfile);
        }
        catch (Exception ex)
        {
            return new AuthResponse(
                Success: false,
                AccessToken: null,
                Message: $"Token refresh failed: {ex.Message}");
        }
    }

    private async Task UpdateRefreshTokenAsync(User user, string oldToken, string newToken)
    {
        // Tìm và cập nhật refresh token cũ
        var oldUserToken = user.Tokens?.FirstOrDefault(t => t.Value == oldToken && t.Type == TokenType.RefreshToken);
        
        if (oldUserToken != null)
        {
            // Cập nhật token cũ thành token mới
            oldUserToken.Value = newToken;
            oldUserToken.ExpiryDate = DateTimeOffset.UtcNow.AddDays(30); // 30 ngày
            
            // TODO: Update via proper UserTokenRepository
            // Hiện tại skip vì chưa có repository cho UserTokens
        }
        else
        {
            // Tạo token mới nếu không tìm thấy (fallback)
            var newUserToken = new UserTokens
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Value = newToken,
                Type = TokenType.RefreshToken,
                CreatedDate = DateTimeOffset.UtcNow,
                ExpiryDate = DateTimeOffset.UtcNow.AddDays(30)
            };

            // TODO: Add via proper UserTokenRepository
            // Hiện tại skip vì chưa có repository cho UserTokens
        }
    }
}
