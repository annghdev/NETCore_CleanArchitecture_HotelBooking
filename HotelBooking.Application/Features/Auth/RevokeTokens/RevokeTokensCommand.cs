namespace HotelBooking.Application.Features.Auth.RevokeTokens;

public record RevokeTokensCommand(Guid UserId) : IRequest;
