using HotelBooking.Application.Features.Users;

namespace HotelBooking.Application.Features.Auth.Common;

public record AuthResponse(
    bool Success,
    string? AccessToken = null, 
    string? RefreshToken = null,
    string? Message = null,
    UserProfileVM? Profile = null);
