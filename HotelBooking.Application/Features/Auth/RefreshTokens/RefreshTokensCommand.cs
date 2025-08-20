using MediatR;

namespace HotelBooking.Application.Features.Auth.RefreshTokens;

public record RefreshTokensCommand(string OldToken) : IRequest<AuthResponse>;
