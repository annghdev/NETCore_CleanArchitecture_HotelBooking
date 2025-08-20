using MediatR;

namespace HotelBooking.Application.Features.Auth.UnlockUser;

public record UnlockAccountCommand(Guid UserId): IRequest;
