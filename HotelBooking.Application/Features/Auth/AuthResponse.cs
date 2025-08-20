using HotelBooking.Application.Features.Users;

namespace HotelBooking.Application.Features.Auth;

public record AuthResponse(
    bool Success,
    string? AccessToken, 
    string? RefreshToken = null, 
    string? Message = null,
    UserProfileVM? Profile = null);
